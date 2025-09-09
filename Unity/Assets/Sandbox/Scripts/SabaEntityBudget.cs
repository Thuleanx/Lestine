namespace Saba {
    [System.Serializable]
	public struct SabaEntityBudget {
        public enum Bucket {
            Stage = 0,
            Teleporter = 1,
            MAX = Teleporter
        }

        public int StageCount;
        public int TeleporterCount;
        public int TotalCount;

        public int GetCount(Bucket bucket) => bucket switch {
            Bucket.Stage => StageCount,
            Bucket.Teleporter => TeleporterCount,
            _ => 0
        };
	}
}
