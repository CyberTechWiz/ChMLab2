using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Globalization;
using NCalc;

namespace AvaloniaApp;

public partial class MainWindow : Window
{
    // Поля для текстовых полей ввода
    private TextBox functionTextBox;
    private TextBox lowerLimitTextBox;
    private TextBox upperLimitTextBox;
    private TextBox epsTextBox;


    // Поля для текстовых полей вывода результатов
    private TextBox trapezoidMethodValue;
    private TextBox simpsonMethodValue;
    private TextBox newtonMethodValue;
    private TextBox trapezoidMethodTime;
    private TextBox simpsonMethodTime;
    private TextBox newtonMethodTime;
    private TextBox trapezoidMethodError;
    private TextBox simpsonMethodError;
    private TextBox newtonMethodError;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // Привязка текстовых полей ввода
            functionTextBox = this.FindControl<TextBox>("FunctionTextBox");
            lowerLimitTextBox = this.FindControl<TextBox>("LowerLimitTextBox");
            upperLimitTextBox = this.FindControl<TextBox>("UpperLimitTextBox");
            epsTextBox = this.FindControl<TextBox>("EpsTextBox");

            // Привязка текстовых полей вывода
            trapezoidMethodValue = this.FindControl<TextBox>("TrapezoidMethodValue");
            simpsonMethodValue = this.FindControl<TextBox>("SimpsonMethodValue");
            newtonMethodValue = this.FindControl<TextBox>("NewtonMethodValue");
            trapezoidMethodTime = this.FindControl<TextBox>("TrapezoidMethodTime");
            simpsonMethodTime = this.FindControl<TextBox>("SimpsonMethodTime");
            newtonMethodTime = this.FindControl<TextBox>("NewtonMethodTime");
            trapezoidMethodError = this.FindControl<TextBox>("TrapezoidMethodError");
            simpsonMethodError = this.FindControl<TextBox>("SimpsonMethodError");
            newtonMethodError = this.FindControl<TextBox>("NewtonMethodError");

            // Привязка кнопок
            var clearButton = this.FindControl<Button>("ClearButton");
            clearButton.Click += ClearButton_Click;

            var calculateButton = this.FindControl<Button>("CalculateButton");
            calculateButton.Click += CalculateButton_Click;
        }

        // Обработчик кнопки "Очистить"
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            functionTextBox.Text = string.Empty;
            lowerLimitTextBox.Text = string.Empty;
            upperLimitTextBox.Text = string.Empty;
            epsTextBox.Text = string.Empty;

            trapezoidMethodValue.Text = string.Empty;
            simpsonMethodValue.Text = string.Empty;
            newtonMethodValue.Text = string.Empty;
            trapezoidMethodTime.Text = string.Empty;
            simpsonMethodTime.Text = string.Empty;
            newtonMethodTime.Text = string.Empty;
            trapezoidMethodError.Text = string.Empty;
            simpsonMethodError.Text = string.Empty;
            newtonMethodError.Text = string.Empty;
        }

        // Обработчик кнопки "Вычислить"
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Считываем данные из текстовых полей
                string function = functionTextBox.Text;
                double lowerLimit = double.Parse(lowerLimitTextBox.Text, CultureInfo.InvariantCulture);
                double upperLimit = double.Parse(upperLimitTextBox.Text, CultureInfo.InvariantCulture);
                double eps = double.Parse(epsTextBox.Text, CultureInfo.InvariantCulture);

                // Вычисляем интеграл разными методами
                (double trapezoidValue, double trapezoidTime, double trapezoidError) = CalculateTrapezoidMethod(function, lowerLimit, upperLimit, eps);
                (double simpsonValue, double simpsonTime, double simpsonError) = CalculateSimpsonMethod(function, lowerLimit, upperLimit, eps);
                (double newtonValue, double newtonTime, double newtonError) = CalculateNewtonMethod(function, lowerLimit, upperLimit, eps);

                // Выводим результаты в текстовые поля
                trapezoidMethodValue.Text = trapezoidValue.ToString();
                trapezoidMethodTime.Text = trapezoidTime.ToString() + " мс";
                trapezoidMethodError.Text = trapezoidError.ToString();

                simpsonMethodValue.Text = simpsonValue.ToString();
                simpsonMethodTime.Text = simpsonTime.ToString() + " мс";
                simpsonMethodError.Text = simpsonError.ToString();

                newtonMethodValue.Text = newtonValue.ToString();
                newtonMethodTime.Text = newtonTime.ToString() + " мс";
                newtonMethodError.Text = newtonError.ToString();
            }
            catch (Exception ex)
            {
                // Обработка ошибок (например, некорректный ввод)
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

    // Метод для вычисления значения функции с использованием NCalc
    private double EvaluateFunction(string function, double x)
    {
        try
        {
            Expression expression = new Expression(function);
            expression.Parameters["x"] = x;

            // Добавляем математические константы
            expression.Parameters["e"] = Math.E;       // e ≈ 2.71828...
            expression.Parameters["pi"] = Math.PI;     // π ≈ 3.14159...

            return Convert.ToDouble(expression.Evaluate());
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка вычисления функции: {ex.Message}");
        }
    }

    //Метод трапеций
    private (double value, double time, double error) CalculateTrapezoidMethod(string function, double a, double b, double eps)
    {
        // Начальное количество интервалов разбиения
        int n = 1;
        // Шаг разбиения
        double h = (b - a) / n;
        // Предыдущее значение интеграла (для контроля точности)
        double prevValue = 0;
        // Текущее значение интеграла, вычисленное по формуле трапеций
        double currentValue = (EvaluateFunction(function, a) + EvaluateFunction(function, b)) * h / 2;

        // Запускаем таймер для измерения времени выполнения
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Цикл для уточнения значения интеграла с увеличением числа интервалов
        while (true)
        {
            // Сохраняем текущее значение интеграла как предыдущее
            prevValue = currentValue;
            // Увеличиваем количество интервалов в 2 раза для повышения точности
            n *= 2;
            // Пересчитываем шаг разбиения
            h = (b - a) / n;
            // Обнуляем текущее значение интеграла
            currentValue = 0;

            // Суммируем площади всех трапеций
            for (int i = 0; i < n; i++)
            {
                // Левая граница текущего интервала
                double x1 = a + i * h;
                // Правая граница текущего интервала
                double x2 = x1 + h;
                // Добавляем площадь текущей трапеции к общему значению
                currentValue += (EvaluateFunction(function, x1) + EvaluateFunction(function, x2)) * h / 2;
            }

            // Проверяем достижение требуемой точности
            if (Math.Abs(currentValue - prevValue) < eps)
                break; // Если точность достигнута, выходим из цикла
        }

        // Останавливаем таймер
        stopwatch.Stop();
        // Вычисляем ошибку как разницу между текущим и предыдущим значением
        double error = Math.Abs(currentValue - prevValue);

        // Возвращаем результат: значение интеграла, время выполнения и ошибку
        return (currentValue, stopwatch.Elapsed.TotalMilliseconds, error);
    }

    //Метод Симпсона
    private (double value, double time, double error) CalculateSimpsonMethod(string function, double a, double b, double eps)
    {
        // Начальное количество интервалов разбиения (должно быть четным)
        int n = 2;
        // Шаг разбиения
        double h = (b - a) / n;
        // Предыдущее значение интеграла (для контроля точности)
        double prevValue = 0;
        // Текущее значение интеграла, вычисленное по формуле Симпсона
        double currentValue = (EvaluateFunction(function, a) + 4 * EvaluateFunction(function, (a + b) / 2) + EvaluateFunction(function, b)) * h / 3;

        // Запускаем таймер для измерения времени выполнения
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Цикл для уточнения значения интеграла с увеличением числа интервалов
        while (true)
        {
            // Сохраняем текущее значение интеграла как предыдущее
            prevValue = currentValue;
            // Увеличиваем количество интервалов в 2 раза для повышения точности
            n *= 2;
            // Пересчитываем шаг разбиения
            h = (b - a) / n;
            // Начинаем вычисление нового значения интеграла
            currentValue = EvaluateFunction(function, a) + EvaluateFunction(function, b);

            // Суммируем значения функции в нечетных точках с коэффициентом 4
            for (int i = 1; i < n; i += 2)
            {
                currentValue += 4 * EvaluateFunction(function, a + i * h);
            }

            // Суммируем значения функции в четных точках с коэффициентом 2
            for (int i = 2; i < n - 1; i += 2)
            {
                currentValue += 2 * EvaluateFunction(function, a + i * h);
            }

            // Умножаем сумму на шаг и делим на 3 (формула Симпсона)
            currentValue *= h / 3;

            // Проверяем достижение требуемой точности
            if (Math.Abs(currentValue - prevValue) < eps)
                break; // Если точность достигнута, выходим из цикла
        }

        // Останавливаем таймер
        stopwatch.Stop();
        // Вычисляем ошибку как разницу между текущим и предыдущим значением
        double error = Math.Abs(currentValue - prevValue);

        // Возвращаем результат: значение интеграла, время выполнения и ошибку
        return (currentValue, stopwatch.Elapsed.TotalMilliseconds, error);
    }

    //Метод Ньютона
    private (double value, double time, double error) CalculateNewtonMethod(string function, double a, double b, double eps)
    {
        // Начальное количество интервалов разбиения (должно быть кратно 3)
        int n = 3;
        // Шаг разбиения
        double h = (b - a) / n;
        // Предыдущее значение интеграла (для контроля точности)
        double prevValue = 0;
        // Текущее значение интеграла, вычисленное по формуле Ньютона (3/8)
        double currentValue = (EvaluateFunction(function, a) + 3 * EvaluateFunction(function, a + h) + 3 * EvaluateFunction(function, a + 2 * h) + EvaluateFunction(function, b)) * 3 * h / 8;

        // Запускаем таймер для измерения времени выполнения
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Цикл для уточнения значения интеграла с увеличением числа интервалов
        while (true)
        {
            // Сохраняем текущее значение интеграла как предыдущее
            prevValue = currentValue;
            // Увеличиваем количество интервалов в 2 раза для повышения точности
            n *= 2;
            // Пересчитываем шаг разбиения
            h = (b - a) / n;
            // Начинаем вычисление нового значения интеграла
            currentValue = EvaluateFunction(function, a) + EvaluateFunction(function, b);

            // Суммируем значения функции в точках с учетом коэффициентов
            for (int i = 1; i < n; i++)
            {
                if (i % 3 == 0)
                    // Для точек, кратных 3, коэффициент 2
                    currentValue += 2 * EvaluateFunction(function, a + i * h);
                else
                    // Для остальных точек коэффициент 3
                    currentValue += 3 * EvaluateFunction(function, a + i * h);
            }

            // Умножаем сумму на шаг и на 3/8 (формула Ньютона)
            currentValue *= 3 * h / 8;

            // Проверяем достижение требуемой точности
            if (Math.Abs(currentValue - prevValue) < eps)
                break; // Если точность достигнута, выходим из цикла
        }

        // Останавливаем таймер
        stopwatch.Stop();
        // Вычисляем ошибку как разницу между текущим и предыдущим значением
        double error = Math.Abs(currentValue - prevValue);

        // Возвращаем результат: значение интеграла, время выполнения и ошибку
        return (currentValue, stopwatch.Elapsed.TotalMilliseconds, error);
    }
}