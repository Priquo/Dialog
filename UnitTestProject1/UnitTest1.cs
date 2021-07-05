using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Dialog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1 //ВНИМАНИЕ!!! Перед началом тестирования закомментируйте, пожалуйста, в классе CriticalPath, строчку №41. Тестовые методы не могут одновременно использовать один и тот же файл :(
    {
        CriticalPath cp = new CriticalPath("../../входные_данные.csv", "newdata.csv"); //перед запуском тестов создайте в папке debug (где exe) файл для записи пустой
        /// <summary>
        /// Проверяет, содержит ли вычисленный критический путь реальный критический путь (который был найден самостоятельно)
        /// </summary>
        [TestMethod]
        public void FindCriticalPathTesting()
        {
            cp.ReadData();
            cp.CalculatePathes();
            CriticalPath.Path p = new CriticalPath.Path() { lastPoint = "7", length = 29, path = "1--3--4--5--6--7" };
            Assert.IsTrue(cp.FindCriticalPath().Contains(p));
        }
        /// <summary>
        /// Проверяет, содержит ли список конечную точку 7, ранее выявленную
        /// </summary>
        [TestMethod]
        public void FindEndingPointTesting()
        {
            cp.ReadData();
            Assert.IsTrue(cp.FindEndingPos().Contains("7"));
        }
        /// <summary>
        /// Проверяет, содержит ли список начальную точку, которая должны быть
        /// </summary>
        [TestMethod]
        public void FindStartingPointTesting()
        {
            cp.ReadData();
            cp.CalculatePathes();
            var dataTest = cp.pathes;
            List<char> tempStartPoint = new List<char>();
            foreach (var p in dataTest)
            {
                tempStartPoint.Add(p.path[0]);
            }
            Assert.IsTrue(tempStartPoint.Contains('1'));
        }
    }
}
