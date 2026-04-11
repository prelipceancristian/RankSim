using RankSim.Core.Matchmaking;

namespace RankSim.Core.Tests.Matchmaking;

public class CombinationHelperTests
{
    public class GetHalfCombinations
    {
        [Theory]
        [InlineData(4, 2, 3)]   // C(3,1) = 3
        [InlineData(6, 3, 10)]  // C(5,2) = 10
        [InlineData(10, 5, 126)] // C(9,4) = 126
        [InlineData(2, 1, 1)]   // C(1,0) = 1
        public void YieldsCorrectCount(int n, int k, int expectedCount)
        {
            var results = CombinationHelper.GetHalfCombinations(n, k).ToList();

            Assert.Equal(expectedCount, results.Count);
        }

        [Fact]
        public void AllResultsContainIndexZero()
        {
            var results = CombinationHelper.GetHalfCombinations(6, 3).ToList();

            Assert.All(results, combo => Assert.Contains(0, combo));
        }

        [Fact]
        public void EachSplitCoversAllIndices()
        {
            const int n = 6;
            const int k = 3;

            foreach (var teamA in CombinationHelper.GetHalfCombinations(n, k))
            {
                var teamB = Enumerable.Range(0, n).Except(teamA).ToList();
                var all = teamA.Concat(teamB).OrderBy(x => x).ToList();

                Assert.Equal(Enumerable.Range(0, n), all);
            }
        }

        [Fact]
        public void NoMirrorDuplicates()
        {
            const int n = 6;
            const int k = 3;
            var seen = new HashSet<string>();

            foreach (var teamA in CombinationHelper.GetHalfCombinations(n, k))
            {
                var teamB = Enumerable.Range(0, n).Except(teamA).OrderBy(x => x).ToArray();
                var keyA = string.Join(",", teamA.OrderBy(x => x));
                var keyB = string.Join(",", teamB);

                // Neither this split nor its mirror should have been seen
                Assert.DoesNotContain(keyA, seen);
                Assert.DoesNotContain(keyB, seen);

                seen.Add(keyA);
            }
        }

        [Fact]
        public void TeamsOfFiveFromTenPlayersYieldsExpectedCombinations()
        {
            // n=10, k=5: index 0 is fixed in every result,
            // remaining 4 slots are chosen from [1..9] in ascending order.
            var expected = new[]
            {
                [0, 1, 2, 3, 4],
                [0, 1, 2, 3, 5],
                [0, 1, 2, 3, 6],
                [0, 1, 2, 3, 7],
                [0, 1, 2, 3, 8],
                [0, 1, 2, 3, 9],
                [0, 1, 2, 4, 5],
                [0, 1, 2, 4, 6],
                [0, 1, 2, 4, 7],
                [0, 1, 2, 4, 8],
                [0, 1, 2, 4, 9],
                [0, 1, 2, 5, 6],
                [0, 1, 2, 5, 7],
                [0, 1, 2, 5, 8],
                [0, 1, 2, 5, 9],
                [0, 1, 2, 6, 7],
                [0, 1, 2, 6, 8],
                [0, 1, 2, 6, 9],
                [0, 1, 2, 7, 8],
                [0, 1, 2, 7, 9],
                [0, 1, 2, 8, 9],
                [0, 1, 3, 4, 5],
                [0, 1, 3, 4, 6],
                [0, 1, 3, 4, 7],
                [0, 1, 3, 4, 8],
                [0, 1, 3, 4, 9],
                [0, 1, 3, 5, 6],
                [0, 1, 3, 5, 7],
                [0, 1, 3, 5, 8],
                [0, 1, 3, 5, 9],
                [0, 1, 3, 6, 7],
                [0, 1, 3, 6, 8],
                [0, 1, 3, 6, 9],
                [0, 1, 3, 7, 8],
                [0, 1, 3, 7, 9],
                [0, 1, 3, 8, 9],
                [0, 1, 4, 5, 6],
                [0, 1, 4, 5, 7],
                [0, 1, 4, 5, 8],
                [0, 1, 4, 5, 9],
                [0, 1, 4, 6, 7],
                [0, 1, 4, 6, 8],
                [0, 1, 4, 6, 9],
                [0, 1, 4, 7, 8],
                [0, 1, 4, 7, 9],
                [0, 1, 4, 8, 9],
                [0, 1, 5, 6, 7],
                [0, 1, 5, 6, 8],
                [0, 1, 5, 6, 9],
                [0, 1, 5, 7, 8],
                [0, 1, 5, 7, 9],
                [0, 1, 5, 8, 9],
                [0, 1, 6, 7, 8],
                [0, 1, 6, 7, 9],
                [0, 1, 6, 8, 9],
                [0, 1, 7, 8, 9],
                [0, 2, 3, 4, 5],
                [0, 2, 3, 4, 6],
                [0, 2, 3, 4, 7],
                [0, 2, 3, 4, 8],
                [0, 2, 3, 4, 9],
                [0, 2, 3, 5, 6],
                [0, 2, 3, 5, 7],
                [0, 2, 3, 5, 8],
                [0, 2, 3, 5, 9],
                [0, 2, 3, 6, 7],
                [0, 2, 3, 6, 8],
                [0, 2, 3, 6, 9],
                [0, 2, 3, 7, 8],
                [0, 2, 3, 7, 9],
                [0, 2, 3, 8, 9],
                [0, 2, 4, 5, 6],
                [0, 2, 4, 5, 7],
                [0, 2, 4, 5, 8],
                [0, 2, 4, 5, 9],
                [0, 2, 4, 6, 7],
                [0, 2, 4, 6, 8],
                [0, 2, 4, 6, 9],
                [0, 2, 4, 7, 8],
                [0, 2, 4, 7, 9],
                [0, 2, 4, 8, 9],
                [0, 2, 5, 6, 7],
                [0, 2, 5, 6, 8],
                [0, 2, 5, 6, 9],
                [0, 2, 5, 7, 8],
                [0, 2, 5, 7, 9],
                [0, 2, 5, 8, 9],
                [0, 2, 6, 7, 8],
                [0, 2, 6, 7, 9],
                [0, 2, 6, 8, 9],
                [0, 2, 7, 8, 9],
                [0, 3, 4, 5, 6],
                [0, 3, 4, 5, 7],
                [0, 3, 4, 5, 8],
                [0, 3, 4, 5, 9],
                [0, 3, 4, 6, 7],
                [0, 3, 4, 6, 8],
                [0, 3, 4, 6, 9],
                [0, 3, 4, 7, 8],
                [0, 3, 4, 7, 9],
                [0, 3, 4, 8, 9],
                [0, 3, 5, 6, 7],
                [0, 3, 5, 6, 8],
                [0, 3, 5, 6, 9],
                [0, 3, 5, 7, 8],
                [0, 3, 5, 7, 9],
                [0, 3, 5, 8, 9],
                [0, 3, 6, 7, 8],
                [0, 3, 6, 7, 9],
                [0, 3, 6, 8, 9],
                [0, 3, 7, 8, 9],
                [0, 4, 5, 6, 7],
                [0, 4, 5, 6, 8],
                [0, 4, 5, 6, 9],
                [0, 4, 5, 7, 8],
                [0, 4, 5, 7, 9],
                [0, 4, 5, 8, 9],
                [0, 4, 6, 7, 8],
                [0, 4, 6, 7, 9],
                [0, 4, 6, 8, 9],
                [0, 4, 7, 8, 9],
                [0, 5, 6, 7, 8],
                [0, 5, 6, 7, 9],
                [0, 5, 6, 8, 9],
                [0, 5, 7, 8, 9],
                new[] { 0, 6, 7, 8, 9 },
            };

            var actual = CombinationHelper.GetHalfCombinations(10, 5).ToList();

            Assert.Equal(expected.Length, actual.Count);
            for (var i = 0; i < expected.Length; i++)
                Assert.Equal(expected[i], actual[i]);
        }

        [Fact]
        public void ReturnsEmptyWhenKGreaterThanN()
        {
            var results = CombinationHelper.GetHalfCombinations(2, 3).ToList();

            Assert.Empty(results);
        }

        [Fact]
        public void ReturnsEmptyWhenKIsZero()
        {
            var results = CombinationHelper.GetHalfCombinations(4, 0).ToList();

            Assert.Empty(results);
        }
    }
}
