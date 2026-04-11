namespace RankSim.Core.Matchmaking;

public static class CombinationHelper
{
    /// <summary>
    /// Yields all ways to split [0..n-1] into two groups of size k,
    /// without mirror duplicates. Fixes index 0 in the chosen set,
    /// then enumerates C(n-1, k-1) combinations from [1..n-1].
    /// </summary>
    public static IEnumerable<int[]> GetHalfCombinations(int n, int k)
    {
        if (k > n || k <= 0)
            yield break;

        // Always place index 0 in team A. This avoids generating mirror duplicates:
        // if {0,1} vs {2,3} is generated, we never also generate {2,3} vs {0,1},
        // because the version without 0 is never emitted as team A.
        // This reduces the output from C(n,k) to C(n-1, k-1) — exactly half the splits.
        var indices = new int[k];
        indices[0] = 0;

        if (k == 1)
        {
            yield return (int[])indices.Clone();
            yield break;
        }

        // Initialize remaining slots to the first valid combination: [0, 1, 2, ..., k-1]
        for (var i = 1; i < k; i++)
            indices[i] = i;

        while (true)
        {
            yield return (int[])indices.Clone();

            // Find the rightmost slot that hasn't reached its ceiling.
            // Each slot `pos` has a maximum value of (n - k + pos).
            // For example with n=6, k=3: slot 1 max=4, slot 2 max=5.
            var pos = k - 1;
            while (pos >= 1 && indices[pos] == n - k + pos)
                pos--;

            // All slots at their ceiling means we've exhausted all combinations.
            if (pos < 1)
                yield break;

            // Increment the found slot, then reset all slots to its right
            // to consecutive values (the smallest valid continuation).
            indices[pos]++;
            for (var i = pos + 1; i < k; i++)
                indices[i] = indices[i - 1] + 1;
        }
    }
}
