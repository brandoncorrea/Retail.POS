﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Retail.POS.Common.Interfaces;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Retail.POS
{
    public partial class POSUI : Form
    {
        private int CurrentQuantity = 1;

        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IItemRepository _itemRepository;
        private readonly IScale _scale;
        private readonly ITransactionHandler _transactionHandler;

        public POSUI(
            IConfiguration config,
            ILogger logger,
            IItemRepository itemRepository,
            IScale scale,
            ITransactionHandler transactionHandler)
        {
            _config = config;
            _logger = logger;
            _itemRepository = itemRepository;
            _scale = scale;
            _transactionHandler = transactionHandler;

            _logger.LogInformation("Initializing POS.");
            InitializeComponent();
            _logger.LogInformation("Initialization Complete.");
        }

        #region Number Pressed Methods
        private void Key_0_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("0");

        private void Key_1_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("1");

        private void Key_2_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("2");

        private void Key_3_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("3");

        private void Key_4_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("4");

        private void Key_5_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("5");

        private void Key_6_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("6");

        private void Key_7_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("7");

        private void Key_8_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("8");

        private void Key_9_Click(object sender, EventArgs e) =>
            AppendItemEntryBox("9");

        private void Key_Back_Click(object sender, EventArgs e)
        {
            if (ItemEntryBox.Text.Length > 0)
                ItemEntryBox.Text = ItemEntryBox.Text[0..^1];
        }

        private void Key_Qty_Click(object sender, EventArgs e)
        {
            string entryBoxValue = ItemEntryBox.Text;
            bool parsed = int.TryParse(entryBoxValue, out int quantity);
            if (!parsed || quantity < 1) return;
            CurrentQuantity = quantity;
            ItemEntryBoxLabel.Text = $"{CurrentQuantity} QTY";
            ItemEntryBox.Clear();
        }

        private void Key_Cancel_Click(object sender, EventArgs e)
        {
            if (ItemEntryBox.Text.Length > 0)
                ItemEntryBox.Text = "";
            else
            {
                ItemEntryBoxLabel.Text = "Enter GTIN or Quantity";
                CurrentQuantity = 1;
            }
        }

        private void AppendItemEntryBox(string value)
        {
            var newValue = ItemEntryBox.Text + value;
            if (newValue.Length <= ItemEntryBox.MaxLength)
                ItemEntryBox.Text = newValue;
        }
        #endregion

        private void Key_Enter_Click(object sender, EventArgs e)
        {
            var gtin = ItemEntryBox.Text;
            if (gtin.Length == 0)
                return;

            var item = new
            {
                ItemId = gtin
            };
            string json = JsonConvert.SerializeObject(item);
            IItem arg = JsonConvert.DeserializeObject<IItem>(json);
            _transactionHandler.Add(arg, 1);
        }

        #region Helpers
        private void ResetTextViews()
        {
            ItemCountLabel.Text = $"{_transactionHandler.Items.Count()} Items";
            GrossTotalLabel.Text = $"${_transactionHandler.GrossTotal:0.00}";
            ItemEntryBoxLabel.Text = "Enter GTIN or Quantity";
            CurrentQuantity = 1;
            ItemEntryBox.Clear();
        }

        private void HighlightLastLine()
        {
            var itemCount = ItemEntryScreen.Items.Count;
            ItemEntryScreen.SetSelected(itemCount - 1, true);
        }
        #endregion

        private void IdleTimer_Tick(object sender, EventArgs e)
        {
            WeightLabel.Text = $"{_scale.Weight:0.00} LB";
        }
    }
}
