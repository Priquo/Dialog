﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dialog
{
    /// <summary>
    /// Форма для реализации интерфейса
    /// </summary>
    public partial class Form1 : Form
    {
        string dataPath; //путь к исходным данным
        /// <summary>
        /// Инициализирует новый экземпляр класса Form1 с загрузкой всех компонентов формы
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Открывает окно для выбора файла с исходными данными, по умолчанию в папке проекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDownload_Click(object sender, EventArgs e)
        {
            var location = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.InitialDirectory = location;
            openFileDialog1.Filter = "csv files (*.csv)|*.csv";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileNames.Length > 1)
                    MessageBox.Show("Нельзя загрузить больше одного файла!");
                else if (openFileDialog1.FileNames.Length == 1)
                    dataPath = openFileDialog1.FileNames[0];
                else
                    MessageBox.Show("Выберите файл!");
            }
        }
        /// <summary>
        /// Открывает окно для выбора файла, куда сохранить данные (либо создание нового). По умолчанию в папке проекта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMakeSolutionAndSave_Click(object sender, EventArgs e)
        {
            Stream myStream;
            var location = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog1.InitialDirectory = location;
            saveFileDialog1.Filter = "csv files (*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    myStream.Close();
                    string savePath = saveFileDialog1.FileName;
                    if (savePath != null && savePath != "")
                    {
                        try
                        {
                            CriticalPath cp = new CriticalPath(dataPath, savePath);
                            cp.MakeResult();
                            MessageBox.Show("Решение сохранено! До свидания");
                            Environment.Exit(0);
                        }
                        catch
                        {
                            MessageBox.Show("Что-то не так! Делайте заново");
                            Environment.Exit(0);
                        }                        
                    }
                }
        }
    }
}
