using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        private static int selected = -2; // index of cardList selected, -1 means search is selected, -2 means name
        private static String searchTerm;

        private static readonly Rectangle listBox = new Rectangle((Game1.START_WIDTH - 500) / 2, (Game1.START_HEIGHT - 700) / 2, 500, 700);
        private static readonly Rectangle searchBox = new Rectangle((Game1.START_WIDTH - 400) / 2, 120, 400, 30);
        private static readonly Rectangle nameBox = new Rectangle((Game1.START_WIDTH - 300) / 2, 20, 300, 60);

        public static void New() {
            name = "Name";
            fileName = null;
            searchTerm = "";

            cardList = new List<CardWithCount>();
            LoadCards();
            SetupStats();
        }

        public static void Edit(String name) {
            SetupStats();
            LoadCards();
            cardList = new List<CardWithCount>();
            searchTerm = "";

            // load deck into cardList while updating stats
            DeckMaker.name = name;
            fileName = name;

            String[] data = File.ReadAllLines("Content\\Decks\\" + name + ".deck");

            foreach(String line in data) {
                String[] split = line.Split(',');
                if(validCards.Contains(split[0])) {
                    Add(new Card(split[0]), int.Parse(split[1]));
                }
            }
        }

        public static void Update() {
            Text.Update();
            Keys key = Text.GetCurrentKey();

            if(selected == -2) {
                // change name
                if(key == Keys.Back && name.Length > 0) {
                    name = name.Substring(0, name.Length - 1);
                } else {
                    name += Text.KeyToLetter(key);
                }
            }
            else if(selected == -1) {
                // type
                // add new card from enter
                if(key == Keys.Enter) {
                    if(validCards.Contains(searchTerm)) {
                        Add(new Card(searchTerm));
                    }
                }
                else if(key == Keys.Tab) {
                    searchTerm = "";
                }
                else if(key == Keys.Back && searchTerm.Length > 0) {
                    searchTerm = searchTerm.Substring(0, searchTerm.Length - 1);
                } else {
                    searchTerm += Text.KeyToLetter(key);
                }
            }
            else {
                // an individual card is selected
                if(key == Keys.OemPlus || key == Keys.Up) {
                    Add(new Card(cardList[selected].Name));
                }
                else if(key == Keys.OemMinus || key == Keys.Down) {
                    Remove(new Card(cardList[selected].Name));
                }
            }

            // change selected item
            if(Game1.JustClicked()) {
                Point mousePos = Mouse.GetState().Position;
                if(searchBox.Contains(mousePos)) {
                    selected = -1;
                }
                else if(nameBox.Contains(mousePos)) {
                    selected = -2;
                }
                else {
                    // check list of cards
                    List<Rectangle> cardBoxes = GetCardBoxes();
                    for(int i = 0; i < cardBoxes.Count; i++) {
                        if(cardBoxes[i].Contains(mousePos)) {
                            selected = i;
                            break;
                        }
                    }
                }
            }
        }

        public static void Draw(SpriteBatch sb) {
            // draw name
            DrawCentered(sb, name, new Vector2(Game1.START_WIDTH / 2, 50));

            // draw card list
            Rectangle shrunk = listBox;
            shrunk.Inflate(-4, -4);
            sb.Draw(Game1.Pixel, listBox, Color.White);
            sb.Draw(Game1.Pixel, shrunk, Color.Black);

            List<Rectangle> listBoxes = GetCardBoxes();
            if(selected == -2) {
                sb.Draw(Game1.Pixel, nameBox, Color.LightBlue * 0.5f);
            }
            else if(selected >= 0) {
                sb.Draw(Game1.Pixel, listBoxes[selected], Color.DarkBlue);
            }

            sb.Draw(Game1.Pixel, searchBox, selected == -1 ? Color.DarkBlue : Color.DarkGray);
            DrawCentered(sb, searchTerm, new Vector2(searchBox.X + searchBox.Width / 2, searchBox.Y + searchBox.Height / 2));
            for(int i = 0; i < cardList.Count; i++) {
                DrawCentered(sb, cardList[i].Name + " x" + cardList[i].Count, new Vector2(listBoxes[i].X + listBoxes[i].Width / 2, listBoxes[i].Y + listBoxes[i].Height / 2));
            }

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
            if(cardList.Count == 0) {
                return;
            }

            StreamWriter output = null; 
            try {
                output = new StreamWriter("Content\\Decks\\" + name + ".deck");

                foreach(CardWithCount card in cardList) {
                    output.WriteLine(card.Name + "," + card.Count);
                }

                // delete old deck if not overwritten
                if (fileName != null && fileName != name) {
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

        public static void Delete() {
            File.Delete("Content\\Decks\\" + fileName + ".deck");
        }

        private static void Add(Card card, int amount = 1) {
            // see if one exists already
            bool added = false;
            for(int i = 0; i < cardList.Count; i++) {
                if(cardList[i].Name == card.Name) {
                    // max of three copies
                    if(cardList[i].Count >= 3) {
                        return;
                    }

                    cardList[i] = new CardWithCount(card.Name, cardList[i].Count + amount);
                    added = true;
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

        private static void Remove(Card card) {
            for(int i = 0; i < cardList.Count; i++) {
                if(cardList[i].Name == card.Name) {
                    cardList[i] = new CardWithCount(card.Name, cardList[i].Count - 1);

                    if(cardList[i].Count <= 0) {
                        // remove from list
                        cardList.RemoveAt(i);
                        selected = -1;
                    }
                    break;
                }
            }

            // add to stats
            total--;
            stats[card.CardType]--;
            curve[card.Cost]--;
        }

        // get the names of all valid cards
        private static void LoadCards() {
            String[] paths = Directory.GetFiles("Content\\Cards");
            validCards = new List<String>();

            foreach(String path in paths) {
                String name = path.Substring(path.LastIndexOf('\\') + 1); // get rid of directories
                name = name.Substring(0, name.IndexOf('.')); // get rid of .card
                validCards.Add(name);
            }
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

        private static List<Rectangle> GetCardBoxes() {
            List<Rectangle> result = new List<Rectangle>();

            for(int i = 0; i < cardList.Count; i++) {
                result.Add(new Rectangle(searchBox.X, searchBox.Y + searchBox.Height * (i+1), searchBox.Width, searchBox.Height));
            }

            return result;
        }
    }
}
