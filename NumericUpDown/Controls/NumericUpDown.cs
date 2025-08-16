using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace NumericUpDown.Controls;

internal class NumericUpDown : TextBox
{
    private int previousValue; // 前回の値を保持

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
        (nameof(Value),
        typeof(int),
        typeof(NumericUpDown),
        new PropertyMetadata(0, OnValueChanged));

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register
        (nameof(Maximum), 
        typeof(int), 
        typeof(NumericUpDown),
        new PropertyMetadata(int.MaxValue));

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register
        (nameof(Minimum),
        typeof(int), 
        typeof(NumericUpDown), 
        new PropertyMetadata(int.MinValue));

    public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register
        (nameof(Increment),
        typeof(int), 
        typeof(NumericUpDown),
        new PropertyMetadata(1));

    static NumericUpDown()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
    }

    /// <summary>
    /// 値 (数値)
    /// </summary>
    public int Value
    { 
        get => (int)GetValue(ValueProperty);
        set
        {
            SetValue(ValueProperty, Clamp(value, previousValue));
            this.SetEnabled();
        }
    }

    /// <summary>
    /// 最大値
    /// </summary>
    public int Maximum 
    { 
        get => (int)GetValue(MaximumProperty); 
        set => SetValue(MaximumProperty, value); 
    }

    /// <summary>
    /// 最小値
    /// </summary>
    public int Minimum 
    { 
        get => (int)GetValue(MinimumProperty); 
        set => SetValue(MinimumProperty, value); 
    }

    /// <summary>
    /// 増分
    /// </summary>
    public int Increment 
    {
        get => (int)GetValue(IncrementProperty); 
        set => SetValue(IncrementProperty, value); 
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_UpButton") is RepeatButton upButton)
        {
            upButton.Click += this.UpButton_Click;
        }

        if (GetTemplateChild("PART_DownButton") is RepeatButton downButton)
        {
            downButton.Click += DownButton_Click;
        }

        if (GetTemplateChild("PART_TextBox") is TextBox textBox)
        {
            textBox.PreviewTextInput += this.OnPreviewTextInput;
            textBox.TextChanged += OnTextChanged;
        }

        this.Unloaded += NumericUpDown_Unloaded;
    }

    /// <summary>
    /// 値が変更されたときの処理
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericUpDown control)
        {
            control.previousValue = control.Value;
        }
    }

    /// <summary>
    /// 入力文字がテキストであるか判定
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static bool IsTextNumeric(string text) => int.TryParse(text, out _) || text == "-";

    private void UpButton_Click(object sender, RoutedEventArgs e)
    {
        Value += Increment;
    }

    private void DownButton_Click(object sender, RoutedEventArgs e)
    {
        Value -= Increment; ;
    }

    /// <summary>
    /// テキスト入力のプレビュー処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextNumeric(e.Text);
    }

    /// <summary>
    /// テキスト変更処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            Value = ParseText(textBox.Text);
        }
    }

    /// <summary>
    /// テキストの解析
    /// </summary>
    /// <param name="text">テキスト</param>
    /// <returns></returns>
    private int ParseText(string text)
    {
        if (GetTemplateChild("PART_TextBox") is not TextBox textBox)
        {
            return previousValue;
        }

        if (!int.TryParse(text, out int result))
        {
            textBox.Text = previousValue.ToString();
            return previousValue;
        }

        return Clamp(result, previousValue);
    }

    /// <summary>
    /// 値を範囲内にクランプする
    /// </summary>
    /// <param name="newValue">新しい値</param>
    /// <param name="previousValue">前回の値</param>
    /// <returns></returns>
    private int Clamp(int newValue, int previousValue)
    {
        if (GetTemplateChild("PART_TextBox") is not TextBox textBox)
        {
            return previousValue;
        }

        if (newValue > Maximum || newValue < Minimum)
        {
            textBox.Text = previousValue.ToString();
            return previousValue; // 範囲外なら前回値を返す
        }

        textBox.Text = newValue.ToString();
        return newValue;
    }

    private void SetEnabled()
    {
        if (GetTemplateChild("PART_UpButton") is RepeatButton upButton)
        {
            upButton.IsEnabled = Value < Maximum;
        }
        if (GetTemplateChild("PART_DownButton") is RepeatButton downButton)
        {
            downButton.IsEnabled = Value > Minimum;
        }
    }

    /// <summary>
    /// アンロード処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDown_Unloaded(object sender, RoutedEventArgs e)
    {
        if (GetTemplateChild("PART_UpButton") is RepeatButton upButton)
        {
            upButton.Click -= this.UpButton_Click;
        }

        if (GetTemplateChild("PART_DownButton") is RepeatButton downButton)
        {
            downButton.Click -= DownButton_Click;
        }

        if (GetTemplateChild("PART_TextBox") is TextBox textBox)
        {
            textBox.PreviewTextInput -= this.OnPreviewTextInput;
            textBox.TextChanged -= OnTextChanged;
        }

        this.Unloaded -= NumericUpDown_Unloaded;
    }
}
