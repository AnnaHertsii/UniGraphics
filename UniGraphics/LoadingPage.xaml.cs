using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UniGraphics
{
    public partial class LoadingPage : Page
    {
        Action loadFinished;
        static readonly int sleepTime = 0;

        public LoadingPage(Action loadFinished)
        {
            this.loadFinished = loadFinished;
            InitializeComponent();
            PickHint();
            SimulateLoading();
        }

        private static readonly string[] hints = {
            "заглянь в меню 'Довідка'! Там є багато корисної інформації.", 
            "ти можеш швидко перемикатись між полями для вводу за допомогою клавіші Tab.",
            "після введення змін в поля натисни курсором по іншому елементу, аби застосувати зміни.",
            "в розділі 'Колірні моделі' зміни яскравості відбуваються без потреби натискати 'Конвертувати'.",
            "побудова фрактала відбуватиметься швидше, якщо вікно буде не великого розміру.",
            "в розділі 'Колірні моделі' ти можеш зберегти конвертоване зображення на свій комп'ютер.",
            "ти можеш зберегти (в меню 'Файл') побудоване зображення фрактала на свій комп'ютер.",
            "в розділі 'Афінні перетворення' червоний круг на фігурі визначає точку, відносно якої здійснюватимуться перетворення.",
            "в розділі 'Афінні перетворення' сітка координатної площини підлаштовується під введені дані."
        };

        private void PickHint()
        {
            Random rand = new Random();
            int hintIndex = rand.Next() % hints.Length;
            HintText.Text = hints[hintIndex];
        }

        private void SimulateLoading()
        {
            Task loadingTask = new Task(() =>
            {
                int percent = 0;
                while (true)
                {
                    ++percent;
                    Dispatcher.Invoke(() => LoadProgress.Value = percent);
                    Thread.Sleep(sleepTime);
                    if (percent == 100)
                        break;
                }
                Dispatcher.Invoke(() => loadFinished());
            });
            loadingTask.Start();
        }
    }
}
