using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialog
{
    /// <summary>
    /// Класс решения задачи критического пути.
    /// </summary>
    public class CriticalPath
    {
        public string dataPath;
        string savePath;
        List<Work> works = new List<Work>(); //Список всех работ (в графике это дуги)
        public List<Path> pathes = new List<Path>(); //Список всех путей        
        struct Work
        {
            public string eventStart, eventEnd;
            public int time;
        }
        public struct Path
        {
            public string path, lastPoint;
            public int length;
        }
        /// <summary>
        /// Инициализирует новый экземпляр класса CriticalPath с сохранением внутренних переменных
        /// </summary>
        /// <param name="dataPath">Путь к файлу входных данных</param>
        /// <param name="savePath">Путь к файлу для сохранения результатов</param>
        public CriticalPath(string dataPath, string savePath)
        {
            this.dataPath = dataPath;
            this.savePath = savePath;
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Debug.Listeners.Add(new TextWriterTraceListener(File.CreateText("../../Ресурсы/listener.txt")));
            Debug.AutoFlush = true;
        }
        /// <summary>
        /// Вычисляет критический путь с вызовом внутренних методов
        /// </summary>
        public void MakeResult()
        {
            ReadData();
            CalculatePathes();
            var criticalPath = FindCriticalPath();
            WriteToFile(criticalPath);
        }
        /// <summary>
        /// Метод поиска критического пути.
        /// </summary>
        /// <returns>Список с перечислением всех вершин</returns>
        public List<Path> FindCriticalPath()
        {
            int maxLength = 0;
            string endPos = FindEndingPos();
            foreach (Path path in pathes.Where(x => x.lastPoint == endPos)) //Проверяет все пути, конечная точка которых совпадает с концом сети
            {
                Debug.WriteLine("Длина текущего пути: " + path.length);
                if (path.length > maxLength) maxLength = path.length; //Вычисляет самый длинный путь из тех, что есть
                Debug.WriteLine("Макс. длина пути: " + maxLength);
            }
            List<Path> criticalPath = new List<Path>();
            foreach (Path path in pathes.Where(x => x.length == maxLength && x.lastPoint == endPos))
            {
                criticalPath.Add(path);
            }
            return criticalPath;
        }
        /// <summary>
        /// Метод подсчета путей.
        /// </summary>
        public void CalculatePathes()
        {
            foreach (Work activity in works.Where(x => x.eventStart == FindStartingPos())) //Сначала в список путей заносятся все начальные дуги
            {
                pathes.Add(new Path { path = activity.eventStart + "--" + activity.eventEnd, lastPoint = activity.eventEnd, length = activity.time });
            }
            for (int i = 0; i < pathes.Count; i++) //Затем идет обход по всем записанным путям (в ходе выполнения цикла их количество пополняется)
            {
                foreach (Work activity in works.Where(x => x.eventStart == pathes[i].lastPoint)) //В список путей заносятся новые пути, которые исходят из проверяемого в данных момент
                {
                    //Таким образом в список заносятся все промежуточные пути
                    pathes.Add(new Path { path = pathes[i].path + "--" + activity.eventEnd, lastPoint = activity.eventEnd, length = pathes[i].length + activity.time });
                }
            }
        }
        /// <summary>
        /// Метод считывания данных из файла.
        /// </summary>
        public void ReadData()
        {
            Debug.WriteLine("Путь для чтения: " + dataPath);
            if (!File.Exists(dataPath))
            {
                MessageBox.Show("Файл не найден!");
                Environment.Exit(0);
            }
            var lines = File.ReadAllLines(dataPath);
            try
            {
                foreach (var line in lines)
                {
                    string[] str = line.Split(';');
                    works.Add(new Work { eventStart = str[0], eventEnd = str[1], time = Convert.ToInt32(str[2]) });
                }
            }
            catch
            {
                MessageBox.Show("Неверный формат записи данных!");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Метод записи в файл.
        /// </summary>
        /// <param name="savingPath">Путь к файлу для сохранения результатов</param>
        void WriteToFile(List<Path> savingPath)
        {
            Debug.WriteLine("Путь для записи: " + savePath);
            if (!File.Exists(savePath)) File.Create(savePath).Close();
            try
            {
                using (StreamWriter sw = new StreamWriter(savePath, false, Encoding.UTF8))
                {
                    if (savingPath.Count == 1)
                    {
                        sw.WriteLine("Найденный критический путь имеет вид:");
                        sw.WriteLine(savingPath[0].path);
                        sw.WriteLine("Его длина составляет: " + savingPath[0].length);
                    }
                    else
                    {
                        sw.WriteLine("Найденные критические пути имеют вид:");
                        foreach (Path savPath in savingPath)
                            sw.WriteLine(savPath.path);
                        sw.WriteLine("Длина каждого из них составляет: " + savingPath[0].length);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Не удалось записать данные в файл!");
                Environment.Exit(0);
            }
        }
        /// <summary>
        /// Метод поиска стартовой точки.
        /// </summary>
        /// <returns>Номер стартовой точки</returns>
        public string FindStartingPos()
        {
            string tempStartPos = " ", lastPoint = "";
            int countCheck = 0;
            foreach (Work activity in works)   //Если нет таких дуг, которые бы входили в данную точку, то она начальная.
            {
                if (works.Where(x => x.eventEnd == activity.eventStart).Count() == 0)
                {
                    tempStartPos = activity.eventStart;
                    countCheck++;
                    Debug.WriteLine("Проверка начальных точек: " + countCheck);
                    if (countCheck > 1 && lastPoint != activity.eventStart)
                    {
                        MessageBox.Show("В введенных данных присутствует несколько начальных точек. Решение невозможно.");
                        Environment.Exit(0);
                    }
                    lastPoint = activity.eventStart;
                }
            }
            if (countCheck == 0)
            {
                MessageBox.Show("Начальная точка отсутствует.");
                Environment.Exit(0);
            }
            return tempStartPos;
        }
        /// <summary>
        /// Метод поиска конечной точки.
        /// </summary>
        /// <returns>Номер конечной точки</returns>
        public string FindEndingPos()
        {
            string tempEndPos = "", lastPoint = "";
            int countCheck = 0;
            foreach (Work activity in works)   //Если нет таких дуг, которые бы исходили из данной точки, то она конечная.
            {
                if (works.Where(x => x.eventStart == activity.eventEnd).Count() == 0)
                {
                    tempEndPos = activity.eventEnd;
                    countCheck++;
                    Debug.WriteLine("Проверка конечных точек: " + countCheck);
                    if (countCheck > 1 && lastPoint != activity.eventEnd)
                    {
                        MessageBox.Show("В введенных данных присутствует несколько конечных точек. Решение невозможно.");
                        Environment.Exit(0);
                    }
                    lastPoint = activity.eventEnd;
                }
            }
            if (countCheck == 0)
            {
                MessageBox.Show("Конечная точка отсутствует.");
                Environment.Exit(0);
            }
            return tempEndPos;
        }                     
    }
}

