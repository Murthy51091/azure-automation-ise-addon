﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Azure.Management.Automation.Models;

namespace AutomationISE
{
    /// <summary>
    /// Interaction logic for RunbookParamDialog.xaml
    /// </summary>
    public partial class RunbookParamDialog : Window
    {
        private IDictionary<string, RunbookParameter> parameterDict;
        private IDictionary<string, string> _paramValues;
        public IDictionary<string, string> paramValues { get { return _paramValues; } }

        public RunbookParamDialog(IDictionary<string, RunbookParameter> parameterDict)
        {
            InitializeComponent();
            this.parameterDict = parameterDict;
            AddParamForms();
        }
        private void AddParamForms()
        {
            /* Update the UI Grid to fit everything */
            for (int i = 0; i < parameterDict.Count * 2; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                rowDef.Height = System.Windows.GridLength.Auto;
                ParametersGrid.RowDefinitions.Add(rowDef);
            }
            Grid.SetRow(ButtonsPanel, ParametersGrid.RowDefinitions.Count - 1);
            /* Fill the UI with parameter data */
            int count = 0;
            foreach (string paramName in parameterDict.Keys)
            {
                /* Parameter Name and Type */
                Label parameterNameLabel = new Label();
                parameterNameLabel.Content = paramName;
                Label parameterTypeLabel = new Label();
                parameterTypeLabel.Content = "(" + parameterDict[paramName].Type + "): ";
                Grid.SetRow(parameterNameLabel, count * 2);
                Grid.SetRow(parameterTypeLabel, count * 2);
                Grid.SetColumn(parameterNameLabel, 0);
                Grid.SetColumn(parameterTypeLabel, 1);
                /* Input field */
                TextBox parameterValueBox = new TextBox();
                parameterValueBox.Name = paramName;
                parameterValueBox.MinWidth = 200;
                parameterValueBox.Margin = new System.Windows.Thickness(0,5,5,5);
                Grid.SetColumn(parameterValueBox, 0);
                Grid.SetRow(parameterValueBox, count * 2 + 1);
                Grid.SetColumnSpan(parameterValueBox, 2);
                /* Add to Grid */
                ParametersGrid.Children.Add(parameterNameLabel);
                ParametersGrid.Children.Add(parameterTypeLabel);
                ParametersGrid.Children.Add(parameterValueBox);
                count++;
            }
        }

        /* 
         * This method assumes that:
         *   1. The window has already been populated with the parameter fields
         *   2. Each input field (text box) has the same name as the parameter it is for
         * 
         */
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            /* Validate parameters and return */
            _paramValues = new Dictionary<string, string>();
            string validationErrors = null;
            foreach (UIElement element in ParametersGrid.Children)
            {
                try
                {
                    TextBox inputField = (TextBox)element;
                    if (String.IsNullOrEmpty(inputField.Text) && parameterDict[inputField.Name].IsMandatory == true)
                        validationErrors += "A value was not provided for the required parameter:  " + inputField.Name + "\r\n";
                    if (!String.IsNullOrEmpty(inputField.Text))
                        _paramValues.Add(inputField.Name, inputField.Text);
                }
                catch { /* not an input field */ }
            }
            if (String.IsNullOrEmpty(validationErrors))
                this.DialogResult = true;
            else
                System.Windows.Forms.MessageBox.Show("Could not submit test job. The following errors were found:\r\n\r\n" + validationErrors);
        }
    }
}
