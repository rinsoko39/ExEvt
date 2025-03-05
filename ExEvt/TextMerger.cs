using System.Collections;

namespace ExEvt;

public class TextMerger: IEnumerable<string> {

    public string[] origin;
    public bool[] remove;
    public List<string>[] add;

    public TextMerger(string[] origin) {
        this.origin = origin;
        remove = new bool[origin.Length];
        add = new List<string>[origin.Length+1];
        for(int i=0; i<=origin.Length; i++)
            add[i] = [];
    }
    
    public TextMerger(string origin): this(origin.Split('\n')) {}

    public void Merge(string[] @new) {
        var lcsLen = GetLcs(origin, @new, out var oriLcs, out var newLcs);
        int oriPrev = -1, newPrev = -1;
        for(int i=0; i<=lcsLen; i++) {
            var oriInd = (i == lcsLen)? origin.Length : oriLcs[i];
            var newInd = (i == lcsLen)? @new.Length : newLcs[i];
            for(int j=oriPrev+1; j<oriInd; j++)
                remove[j] = true;
            for(int j=newPrev+1; j<newInd; j++)
                add[oriInd].Add(@new[j]);
            oriPrev = oriInd;
            newPrev = newInd;
        }
    }

    public void Merge(string @new) => Merge(@new.Split('\n'));

    public IEnumerator<string> GetEnumerator() {
        for(int i=0; i<=origin.Length; i++) {
            foreach(var line in add[i])
                yield return line;
            if(i == origin.Length)
                yield break;
            if(remove[i])
                continue;
            yield return origin[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => string.Join("\n", this);

    private class LcsData {
        public int len;
        public int befInd, aftInd;
        public LcsData? prev;
        private LcsData() {}
        public LcsData(LcsData prev, int befInd, int aftInd) {
            len = prev.len + 1;
            this.befInd = befInd;
            this.aftInd = aftInd;
            this.prev = prev;
        }
        public static LcsData Zero { get; } = new();
        public static LcsData Max(LcsData a, LcsData b)
            => a.len >= b.len? a : b;
    }

    private static int GetLcs(string[] before, string[] after, out int[] LcsInBef, out int[] LcsInAft) {
        var f = new LcsData[2, after.Length];
        LcsData At(int i, int j) => (i < 0 || j < 0)? LcsData.Zero : f[i&1, j];
        for(int i=0; i<before.Length; i++) {
            for(int j=0; j<after.Length; j++) {
                f[i&1, j] = LcsData.Max(At(i-1, j), At(i, j-1));
                if(before[i] == after[j])
                    f[i&1, j] = new(At(i-1, j-1), i, j);
            }
        }
        var res = At(before.Length-1, after.Length-1);
        var len = res.len;
        LcsInBef = new int[len]; LcsInAft = new int[len];
        var cur = res;
        for(int i=len-1; i>=0; i--) {
            LcsInBef[i] = cur.befInd;
            LcsInAft[i] = cur.aftInd;
            cur = cur.prev!;
        }
        return len;
    }

}
