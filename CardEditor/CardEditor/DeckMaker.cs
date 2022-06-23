using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardEditor
{
    static class DeckMaker
    {

        private static List<String> validCards;
        private static Dictionary<Card.Type, int> stats; // number of each type
        private static int total;

        private static List<CardWithCount> cardList;

        public static void New() {
            cardList = new List<CardWithCount>();
            LoadCards();
            SetupStats();
        }

        public static void Edit(String name) {
            SetupStats();
            LoadCards();

            // load deck into cardList while updating stats

            // check for deleted cards
        }

        public static void Update() {
            // change name

            // select card for +/-

            // add new card
        }

        public static void Draw() {
            // draw name

            // draw
        }

        public static void Save() {

        }

        // get the names of all valid cards
        private static void LoadCards() {

        }

        private static void SetupStats() {
            total = 0;
            stats = new Dictionary<Card.Type, int>();
            stats[Card.Type.Fortress] = 0;
            stats[Card.Type.Monster] = 0;
            stats[Card.Type.Effect] = 0;
            stats[Card.Type.Environment] = 0;
            stats[Card.Type.Structure] = 0;
        }

        private struct CardWithCount {
            public String Name;
            public int Count;
        }
    }
}
