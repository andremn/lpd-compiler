using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LPD.Compiler.Controls
{
    public class AttachedProperties
    {
        private static int _previousLineCount = 0;

        public static string GetBindableLineCount(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableLineCountProperty);
        }

        public static void SetBindableLineCount(DependencyObject obj, string value)
        {
            obj.SetValue(BindableLineCountProperty, value);
        }

        // Using a DependencyProperty as the backing store for BindableLineCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindableLineCountProperty =
            DependencyProperty.RegisterAttached(
            "BindableLineCount",
            typeof(string),
            typeof(AttachedProperties),
            new UIPropertyMetadata("1"));
        
        
        public static bool GetHasBindableLineCount(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasBindableLineCountProperty);
        }

        public static void SetHasBindableLineCount(DependencyObject obj, bool value)
        {
            obj.SetValue(HasBindableLineCountProperty, value);
        }

        // Using a DependencyProperty as the backing store for HasBindableLineCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasBindableLineCountProperty =
            DependencyProperty.RegisterAttached(
            "HasBindableLineCount",
            typeof(bool),
            typeof(AttachedProperties),
            new UIPropertyMetadata(
                false,
                new PropertyChangedCallback(OnHasBindableLineCountChanged)));

        private static void OnHasBindableLineCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)o;

            if ((e.NewValue as bool?) == true)
            {
                _previousLineCount = textBox.LineCount;
                textBox.TextChanged += OnTextChanged;

                if (_previousLineCount > 0)
                {
                    textBox.SetValue(BindableLineCountProperty, _previousLineCount.ToString());
                }
            }
            else
            {
                textBox.TextChanged -= OnTextChanged;
            }
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var currentLineCount = textBox.LineCount;

            if (currentLineCount == _previousLineCount || currentLineCount == -1)
            {
                return;
            }

            string x = string.Empty;

            for (int i = 0; i < currentLineCount; i++)
            {
                x += i + 1 + "\n\n";
            }

            textBox.SetValue(BindableLineCountProperty, "\n" + x);
            _previousLineCount = currentLineCount;
        }
    }
}
