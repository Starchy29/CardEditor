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
    static class Gallery
    {
        private static List<Card> cards;
        private static List<Rectangle> rects;
        private static int scroll = 0;
        private static int lastMouseScroll; // for detecting scroll changes

        private const int CARD_GAP = 50; // space between cards
        private const int PER_ROW = 4;
        private const int WIDTH = 250;

        public static bool JustViewing = false;

        public static void Load(Card.Type type) {
            String[] paths = Directory.GetFiles("Content\\Cards");
            cards = new List<Card>();

            foreach(String path in paths) {
                String name = path.Substring(path.LastIndexOf('\\') + 1); // get rid of directories
                name = name.Substring(0, name.IndexOf('.')); // get rid of .card
                Card newCard = new Card(name);
                if(newCard.CardType == type) {
                    cards.Add(newCard);
                }
            }
            Sort();

            SetupRectangles();
        }

        public static void LoadDeck(List<CardWithCount> cardList) {
            cards = new List<Card>();

            foreach(CardWithCount cardBundle in cardList) {
                for(int i = 0; i < cardBundle.Count; i++) {
                    cards.Add(new Card(cardBundle.Name));
                }
            }

            Sort();

            SetupRectangles();
        }

        public static Card.Type GetCurrentType() {
            if(cards.Count > 0) {
                return cards[0].CardType;
            }

            return Card.Type.Fortress;
        }

        public static void Remove(String cardName) {
            for(int i = 0; i < cards.Count; i++) {
                if(cards[i].Name == cardName) {
                    cards.RemoveAt(i);
                    break;
                }
            }

            rects.RemoveAt(rects.Count - 1);
        }

        public static void Add(String cardName) {
            cards.Add(new Card(cardName));
            Sort();

            int height = Card.GetHeight(WIDTH);
            int nextCard = cards.Count - 1;
            rects.Add(new Rectangle(350 + (WIDTH + CARD_GAP) * (nextCard % PER_ROW), 50 + (CARD_GAP + height) * (nextCard / PER_ROW), WIDTH, height));
        }

        public static void Update() {
            // edit a card if it is clicked
            MouseState mouse = Mouse.GetState();
            if(!JustViewing) { 
                if(Game1.JustClicked()) {
                    for(int i = 0; i < cards.Count; i++) {
                        Rectangle tester = rects[i];
                        tester.Y -= scroll;
                        if(tester.Contains(mouse.Position)) {
                            Game1.Edit(cards[i].Name);
                        }
                    }
                }
            }

            // scroll
            scroll += lastMouseScroll - mouse.ScrollWheelValue;
            int maxScroll = (Card.GetHeight(WIDTH) + CARD_GAP) * ((int)Math.Ceiling((double)cards.Count / PER_ROW) - 1);
            if(scroll < 0) {
                scroll = 0;
            }
            else if(scroll > maxScroll) {
                scroll = maxScroll;
            }
            lastMouseScroll = mouse.ScrollWheelValue;
        }

        public static void Draw(SpriteBatch sb) {
            for(int i = 0; i < cards.Count; i++) {
                Rectangle shifted = rects[i];
                shifted.Y -= scroll;
                cards[i].Draw(sb, shifted);
            }
        }

        private static void SetupRectangles() {
            rects = new List<Rectangle>();
            int height = Card.GetHeight(WIDTH);
            for(int i = 0; i < cards.Count; i++) {
                rects.Add(new Rectangle(350 + (WIDTH + CARD_GAP)* (i % PER_ROW), 50 + (CARD_GAP + height)* (i / PER_ROW), WIDTH, height)); 
            }
        }

        private static void Sort() {
            // sort by cost least to greatest
            cards.Sort((Card first, Card last) => { return first.Cost - last.Cost; });
        }
    }
}
