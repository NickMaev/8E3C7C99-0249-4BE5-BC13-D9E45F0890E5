using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Triangle.Core;

namespace Triangle.Tests
{
    [TestClass]
    public class SummFinderTestClass
    {
        private FileInfo FileWithInvalidData =>  new FileInfo("./inputs/emptyInput.txt");
        private FileInfo FileWithValidData1 => new FileInfo("./inputs/4_successPaths.maxSumm_1512.215_192_269_836.txt");
        private FileInfo UnexistantFile => new FileInfo("lol.txt");
        private FileInfo FileWithValidData2 => new FileInfo("./inputs/1_successPaths.maxSumm_842.3_504_117_218.txt");

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowIfObjectNotInitialized()
        {
            var s = new SummFinder();

            s.GetMaxSumm(out int[] path);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InputArrayIsNull()
        {
            var s = new SummFinder();

            s.LoadFromArray(null as int[][]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void InputArrayIsEmpty()
        {
            int[][] arr = new int[0][];
            var s = new SummFinder();

            s.LoadFromArray(arr);
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void InputDataIsEmptyFromFile()
        {
            var s = new SummFinder();

            s.LoadFromFile(FileWithInvalidData);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void InputDataFromUnexistingFile()
        {
            var s = new SummFinder();

            s.LoadFromFile(UnexistantFile);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void FirstValueOfTriangleIsEven()
        {
            var array = new int[1][];
            array[0] = new int[1];
            array[0][0] = 2;
            var s = new SummFinder();
            s.LoadFromArray(array);

            s.GetMaxSumm(out int[] maxSummPath);
        }

        [TestMethod]
        public void FirstValueOfTriangleIsOdd()
        {
            var array = new int[1][];
            array[0] = new int[1];
            array[0][0] = 3;
            var s = new SummFinder();
            s.LoadFromArray(array);

            var result = s.GetMaxSumm(out int[] maxSummPath);
            
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void TriangleHasNoWayToTheEnd()
        {
            // 1
            // 3 3
            var array = new int[2][];
            array[0] = new int[1];
            array[1] = new int[2];
            array[0][0] = 1;
            array[1][0] = 3;
            array[1][1] = 3;

            var s = new SummFinder();
            s.LoadFromArray(array);

            var result = s.GetMaxSumm(out int[] maxSummPath);
        }

        [TestMethod]
        public void TraveledAllPaths()
        {
            var s = new SummFinder();
            s.LoadFromFile(FileWithValidData1);

            var result = s.GetMaxSumm(out int[] maxSummPath);

            Assert.AreEqual(4, s.SuccessfulPathCount);
        }

        [TestMethod]
        public void FindPathWithMaxSumm()
        {
            var s = new SummFinder();
            s.LoadFromFile(FileWithValidData1);

            var path = new int[] {215, 192, 269, 836};
            var result = s.GetMaxSumm(out int[] maxSummPath);

            for (int i = 0; i < path.Length; i++)
            {
                Assert.AreEqual(path[i], maxSummPath[i]);
            }
        }
        
        [TestMethod]
        public void SummOfRightPathIsCorrect()
        {
            var s = new SummFinder();
            s.LoadFromFile(FileWithValidData1);

            var result = s.GetMaxSumm(out int[] maxSummPath);

            Assert.AreEqual(result, maxSummPath.Sum());
        }

        [TestMethod]
        public void AllAvailableChoisesIsUsed()
        {
            var s = new SummFinder();
            s.LoadFromFile(FileWithValidData1);

            var result = s.GetMaxSumm(out int[] maxSummPath);

            Assert.AreEqual(s.PathsWithAvailableChoice.Count, 0);
        }

        [TestMethod]
        public void IsInstanceReusable()
        {
            var s = new SummFinder();
            s.LoadFromFile(FileWithValidData1);

            var result = s.GetMaxSumm(out int[] maxSummPath);

            var path = new int[] {3, 504, 117, 218};
            s.LoadFromFile(FileWithValidData2);

            result = s.GetMaxSumm(out maxSummPath);

            Assert.AreEqual(result, 842);
            for (int i = 0; i < path.Length; i++)
            {
                Assert.AreEqual(path[i], maxSummPath[i]);
            }
        }
    }
}
