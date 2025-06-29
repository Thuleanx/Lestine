using UnityEngine;
using System.Collections.Generic;
using System;
using Dan.Main;
using PrettyPatterns;

namespace Netcode {

[CreateAssetMenu(fileName = "Leaderboard",
    menuName = "~/Leaderboard", order = 1)]
public class SimpleLeaderboard : ScriptableObject {
    public string publicKey;

    public struct Record {
        public string Name;
        public int Rank;
        public int Score;
    }

    public (bool, Record) Parse(Dan.Models.Entry entry) {
        string entryData = entry.Extra;
        Record record = new Record() {
            Name = entry.Username,
            Rank = entry.Rank,
            Score = entry.Score
        };

        return (true, record);
    }

    public void Save(Record record, Action<bool> regularCallback, Action<string> errorCallback) {
        LeaderboardCreator.UploadNewEntry(
            publicKey,
            username: record.Name,
            score: record.Score,
            regularCallback,
            errorCallback
        );
        /* LeaderboardCreator.ResetPlayer(() => { */
        /* }); */
    }

    public void Load(int numberOfEntries, Action<List<Record>> callback) {
        LeaderboardCreator.GetLeaderboard(
            publicKey,
            isInAscendingOrder: false,
            searchQuery: new Dan.Models.LeaderboardSearchQuery() {
                Skip = 0,
                Take = numberOfEntries
            },
            callback: (entries) => {
                List<Record> records = new List<Record>();
                for (int i = 0; i < entries.Length; i++) {
                    var (successful, value) = Parse(entries[i]);
                    records.Add(value);
                }
                callback?.Invoke(records);
            }
        );
    }

    public void Query(string Username, Action<Optional<Record>> callback) {
        LeaderboardCreator.GetLeaderboard(
            publicKey,
            isInAscendingOrder: false,
            searchQuery: new Dan.Models.LeaderboardSearchQuery() {
                Username = Username
            },
            callback: (entries) => {
                if (entries.Length > 0) {
                    var (successful, value) = Parse(entries[0]);
                    if (successful) {
                        callback?.Invoke(new Optional<Record>(value));
                        return;
                    }
                }
                callback?.Invoke(new Optional<Record>());
            }
        );
    }
}

}
