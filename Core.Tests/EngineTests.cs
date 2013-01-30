using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Core;
using Core.Domain;

namespace Core.Tests
{
    [TestFixture]
    public class EngineTests
    {
        #region Testwide Members
        private List<Generation<SimpleCell>> _testRuns = new List<Generation<SimpleCell>>();
        #endregion

        #region Instantiation Tests

        [Test]
        public void DefaultInstantiationTest()
        {
            var engine = new Engine<SimpleCell>();
            Assert.IsNotNull(engine.CurrentGeneration);
            Assert.IsNotNull(engine.NextGeneration);
            Assert.AreEqual(0, engine.MaxGenerations);
        }

        [Test]
        public void InstantiationWithSeedOnlyDefaultsMaxGenerationsToFive()
        {
            var engine = new Engine<SimpleCell>(new Generation<SimpleCell>());

            Assert.IsNotNull(engine, "Instantiation with seed only failed");
            Assert.AreEqual(5, engine.MaxGenerations);
        }

        [Test]
        public void InstantiationWithUserSuppliedGenCount()
        {
            var engine = new Engine<SimpleCell>(new Generation<SimpleCell>(), 10);

            Assert.AreEqual(10, engine.MaxGenerations, "User supplied generation count not set properly");
        }

        #endregion

        #region Process Tests

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "Something is not setup right. Recheck seed.")]
        public void ProcessThrowsExceptionOnSanityCheckFailure()
        {
            var engine = new Engine<SimpleCell>(); //Current gen is just new, sanity will fail
            engine.Process(Help);
        }

        //Test pattern Period 2 Blinker - Oscillator
        [Test]
        public void ProcessOscillatorTest()
        {
            Generation<SimpleCell> seedGeneration = new Generation<SimpleCell>(5, 5);
            //setup seed
            //Blinker (period 2) - Oscillator
            seedGeneration.Cells[2][1].IsAlive = true;
            seedGeneration.Cells[2][2].IsAlive = true;
            seedGeneration.Cells[2][3].IsAlive = true;

            var engine = new Engine<SimpleCell>(seedGeneration, 2);
            engine.DontSleep = true;
            engine.Process(Help);

            Assert.AreEqual(3, _testRuns.Count);

            var run0 = _testRuns[0]; //this is the seed
            Assert.IsTrue(run0.Cells[2][1].IsAlive);
            Assert.IsTrue(run0.Cells[2][2].IsAlive);
            Assert.IsTrue(run0.Cells[2][3].IsAlive);

            var run1 = _testRuns[1];

            Assert.IsFalse(run1.Cells[0][0].IsAlive);
            Assert.IsFalse(run1.Cells[0][1].IsAlive);
            Assert.IsFalse(run1.Cells[0][2].IsAlive);
            Assert.IsFalse(run1.Cells[0][3].IsAlive);
            Assert.IsFalse(run1.Cells[0][4].IsAlive);

            Assert.IsFalse(run1.Cells[1][0].IsAlive);
            Assert.IsFalse(run1.Cells[1][1].IsAlive);
            Assert.IsTrue(run1.Cells[1][2].IsAlive);
            Assert.IsFalse(run1.Cells[1][3].IsAlive);
            Assert.IsFalse(run1.Cells[1][4].IsAlive);

            Assert.IsFalse(run1.Cells[2][0].IsAlive);
            Assert.IsFalse(run1.Cells[2][1].IsAlive);
            Assert.IsTrue(run1.Cells[2][2].IsAlive);
            Assert.IsFalse(run1.Cells[2][3].IsAlive);
            Assert.IsFalse(run1.Cells[2][4].IsAlive);

            Assert.IsFalse(run1.Cells[3][0].IsAlive);
            Assert.IsFalse(run1.Cells[3][1].IsAlive);
            Assert.IsTrue(run1.Cells[3][2].IsAlive);
            Assert.IsFalse(run1.Cells[3][3].IsAlive);
            Assert.IsFalse(run1.Cells[3][4].IsAlive);

            Assert.IsFalse(run1.Cells[4][0].IsAlive);
            Assert.IsFalse(run1.Cells[4][1].IsAlive);
            Assert.IsFalse(run1.Cells[4][2].IsAlive);
            Assert.IsFalse(run1.Cells[4][3].IsAlive);
            Assert.IsFalse(run1.Cells[4][4].IsAlive);

            var run2 = _testRuns[2]; //this is same as seed due to oscillation effect
            Assert.IsTrue(run2.Cells[2][1].IsAlive);
            Assert.IsTrue(run2.Cells[2][2].IsAlive);
            Assert.IsTrue(run2.Cells[2][3].IsAlive);

            //the asserts above are too verbose. In the next version, compact them to another method that can
            //assert bool values in an array and a list of indices
        }

        #endregion

        #region Helper
        private void Help(Generation<SimpleCell> g, int level) 
        {
            _testRuns.Add(g);
            //if need be uncomment the code below before running the tests
            //for (int outerCtr = 0; outerCtr < g.Cells.Count; outerCtr++)
            //{
            //    string[] line = g.Cells[outerCtr].Select(c => c.IsAlive ? "X" : "-").ToArray<string>();
            //    foreach (string part in line)
            //        Console.Write(part);
            //    Console.WriteLine();
            //}
            //Console.WriteLine();

        }
        #endregion
    }

    [TestFixture]
    public class RuleProcessorTests
    {
        #region ShouldCellDie Tests

        //Checking all rules in one test, just being lazy. Ideally we would have one test for each rule
        //I'll excuse myself this time.
        [Test]
        public void TestRules()
        {
            //1. Any live cell with fewer than two live neighbours dies, as if caused by under-population.
            Assert.IsTrue(RuleProcessor.ShouldCellDie(1, true), "Rule violated: Any live cell with fewer than two live neighbours dies, as if caused by under-population.");

            //2. Any live cell with two or three live neighbours lives on to the next generation.
            //test two
            Assert.IsFalse(RuleProcessor.ShouldCellDie(2, true), "Rule violated: Any live cell with two or three live neighbours lives on to the next generation.[Case 2 neighbours]");
            //test three
            Assert.IsFalse(RuleProcessor.ShouldCellDie(3, true), "Rule violated: Any live cell with two or three live neighbours lives on to the next generation.[Case 3 neighbours]");

            //3. Any live cell with more than three live neighbours dies, as if by overcrowding.
            Assert.IsTrue(RuleProcessor.ShouldCellDie(1, true), "Rule violated: Any live cell with more than three live neighbours dies, as if by overcrowding.");

            //4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            Assert.IsFalse(RuleProcessor.ShouldCellDie(3, false), "Rule violated: Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.");

            //5. By default all dead cells stay put
            Assert.IsTrue(RuleProcessor.ShouldCellDie(5, false), "Rule violated: By default all dead cells should stay put.");
        }

        #endregion
    }
}
