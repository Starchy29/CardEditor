using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CardEditor
{
    public struct CardWithCount {
        public String Name;
        public int Count;

        public CardWithCount(String name, int count) {
            Name = name;
            Count = count;
        }
    }

    static class DeckMaker
    {

        private static List<String> validCards;

        private static Dictionary<Card.Type, int> stats; // number of each type
        private static int[] curve; // number of cards of each cost, index is cost
        private static int total;

        private static List<CardWithCount> cardList;
        private static String name;
        private static String fileName;

        private static int selected; // index of cardList selected, -1 means search is selected, -2 means name

        private static readonly Rectangle listBox = new Rectangle((Game1.START_WIDTH - 500) / 2, (Game1.START_HEIGHT - 700) / 2, 500, 700);

        public static void New() {
            name = "Name";

            cardList = new List<CardWithCount>();
            LoadCards();
            SetupStats();
        }

        public static void Edit(String name) {
            SetupStats();
            LoadCards();

            // load deck into cardList while updating stats
            DeckMaker.name = name;

            String[] data = File.ReadAllLines("Content\\Decks\\" + name + ".deck");

            foreach(String line in data) {
                String[] split = line.Split(',');

            }

            // check for deleted cards
        }

        public static void Update() {
            if(selected == -2) {
                // change name
            }
            else if(selected == -1) {
                // add new card
            }


            // select card for +/-

            // change selected item
        }

        public static void Draw(SpriteBatch sb) {
            // draw name
            DrawCentered(sb, name, new Vector2(Game1.START_WIDTH / 2, 50));

            // draw card list
            Rectangle shrunk = listBox;
            shrunk.Inflate(-4, -4);
            sb.Draw(Game1.Pixel, listBox, Color.White);
            sb.Draw(Game1.Pixel, shrunk, Color.Black);

            // draw stats
            Rectangle statsBox = new Rectangle(1150, (Game1.START_HEIGHT - 600) / 2, 400, 600);
            shrunk = statsBox;
            shrunk.Inflate(-4, -4);
            sb.Draw(Game1.Pixel, statsBox, Color.White);
            sb.Draw(Game1.Pixel, shrunk, Color.Black);

            DrawCentered(sb, "Total: " + total, new Vector2(statsBox.X + statsBox.Width / 2, statsBox.Y + (40 * 1)));
            DrawCentered(sb, "Monster: " + stats[Card.Type.Monster], new Vector2(statsBox.X + statsBox.Width / 2, statsBox.Y + (40 * 2)));
            DrawCentered(sb, "Structure: " + stats[Card.Type.Structure], new Vector2(statsBox.X + statsBox.Width / 2, statsBox.Y + (40 * 3)));
            DrawCentered(sb, "Environment: " + stats[Card.Type.Environment], new Vector2(statsBox.X + statsBox.Width / 2, statsBox.Y + (40 * 4)));
            DrawCentered(sb, "Effect: " + stats[Card.Type.Effect], new Vector2(statsBox.X + statsBox.Width / 2, statsBox.Y + (40 * 5)));

            int barWidth = 20;
            int barStep = statsBox.Width / 6;
            for(int i = 0; i < 5; i++) {
                int barMid = statsBox.X + (i + 1) * barStep;
                DrawCentered(sb, "" + (i + 1), new Vector2(barMid, statsBox.Y + statsBox.Height - 40));

                int baseY = statsBox.Y + statsBox.Height - 60;
                int height = curve[i] * 10;
                sb.Draw(Game1.Pixel, new Rectangle(barMid - barWidth / 2, baseY - height, barWidth, height), Color.White);
            }
        }

        public static void View() {
            // pass card list to gallery
            Gallery.LoadDeck(cardList);
        }

        public static void Save() {
            StreamWriter output = null; 
            try {
                output = new StreamWriter("Content\\Decks\\" + name + ".deck");

                foreach(CardWithCount card in cardList) {
                    output.WriteLine(card.Name + "," + card.Count);
                }

                // delete old card if not overwritten
                if (fileName != name) {
                    File.Delete("Content\\Decks\\" + fileName + ".deck");
                    fileName = name;
                }
            }
            catch(Exception e) {
                // report that save failed?
            }
            finally {
                if(output != null) {
                    output.Close();
                }
            }
        }

        private static void Add(Card card, int amount = 1) {
            // see if one exists already
            bool added = false;
            for(int i = 0; i < cardList.Count; i++) {
                if(cardList[i].Name == card.Name) {
                    cardList[i] = new CardWithCount(card.Name, cardList[i].Count + amount);
                    break;
                }
            }

            // add if it is a new card
            if(!added) {
                cardList.Add(new CardWithCount(card.Name, amount));
            }

            // add to stats
            total++;
            stats[card.CardType] += amount;
            curve[card.Cost] += amount;
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

            curve = new int[5]; // in game rules, no card costs more than 5
        }

        private static void DrawCentered(SpriteBatch sb, String text, Vector2 center) {
            Vector2 dims = Game1.Font.MeasureString(text);
            sb.DrawString(Game1.Font, text, center - dims / 2, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }
}
