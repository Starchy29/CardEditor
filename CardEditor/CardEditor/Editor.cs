using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardEditor
{
    static class Editor
    {
        private enum Trait { // the things to edit about the card
            Name,
            Cost,
            Attack,
            Health,
            Text
        }

        private static Card card; // the card being edited
        private static Trait selected = Trait.Name;

        private static Dictionary<Trait, Rectangle> hitBoxes;

        private static Rectangle textBoxEditor = new Rectangle(1150, 150, 400, 600);
        private static List<String> letters = new List<String>();
        private static int cursor = 0; // index of the next letter to write

        private const int letDim = 20; // width / height of text box editor letters

        public static void Load(String name) {
            card = new Card(name);

            letters.Clear();
            String textBox = card.Text;
            for(int i = 0; i < textBox.Length; i++) {
                letters.Add("" + textBox[i]);
            }
            cursor = letters.Count;
            selected = Trait.Name;
        }

        public static void New(Card.Type type) {
            card = new Card(type);
            letters.Clear();
            cursor = 0;
            selected = Trait.Name;
        }

        public static void Update() {
            if(hitBoxes == null) {
                hitBoxes = new Dictionary<Trait, Rectangle>();
                hitBoxes[Trait.Name] = new Rectangle(540, 60, 520, 80);
                hitBoxes[Trait.Cost] = new Rectangle(500, 740, 120, 120);
                hitBoxes[Trait.Attack] = new Rectangle(840, 740, 120, 120);
                hitBoxes[Trait.Health] = new Rectangle(980, 740, 120, 120);
                hitBoxes[Trait.Text] = textBoxEditor;
            }

            Text.Update();
            Keys key = Text.GetCurrentKey();
            if(key != Keys.None) {
                switch(selected) {
                    case Trait.Name:
                        if(key == Keys.Back) {
                            String name = card.Name;
                            if(name.Length > 0) {
                                card.Name = name.Substring(0, name.Length - 1);
                            }
                        } else {
                            card.Name = card.Name + Text.KeyToLetter(key); // add character to name
                        }
                        break;

                    case Trait.Attack:
                        int newAttack;
                        if(int.TryParse(Text.KeyToLetter(key), out newAttack)) {
                            card.Attack = newAttack;
                        }
                        break;

                    case Trait.Health:
                        int newHealth;
                        if(int.TryParse(Text.KeyToLetter(key), out newHealth)) {
                            card.Health = newHealth;
                        }
                        break;

                    case Trait.Cost:
                        int newCost;
                        if(int.TryParse(Text.KeyToLetter(key), out newCost)) {
                            card.Cost = newCost;
                        }
                        break;

                    // most complicated thing to edit here
                    case Trait.Text:
                        // type a key
                        if(key == Keys.Back) {
                            if(letters.Count > 0 && cursor > 0) {
                                letters.RemoveAt(cursor - 1);
                                cursor--;
                            }
                        }
                        else if(key == Keys.Up) {
                            cursor -= 20;
                            if(cursor < 0) {
                                cursor = 0;
                            }
                        }
                        else if(key == Keys.Left) {
                            cursor -= 1;
                            if(cursor < 0) {
                                cursor = 0;
                            }
                        }
                        else if(key == Keys.Down) {
                            cursor += 20;
                            if(cursor > letters.Count) {
                                cursor = letters.Count;
                            }
                        }
                        else if(key == Keys.Right) {
                            cursor += 1;
                            if(cursor > letters.Count) {
                                cursor = letters.Count;
                            }
                        }
                        else {
                            String inserter = Text.KeyToLetter(key);
                            if(inserter.Length > 0) {
                                letters.Insert(cursor, inserter); // add character to name
                                cursor++;
                            }
                        }
                        break;
                }
            }

            // Special text box stuff
            if(selected == Trait.Text) {
                // change cursor position
                if(Mouse.GetState().LeftButton == ButtonState.Pressed && textBoxEditor.Contains(Mouse.GetState().Position)) {
                    Vector2 pos = Mouse.GetState().Position.ToVector2();
                    pos -= new Vector2(textBoxEditor.X, textBoxEditor.Y); // relative to text box top-left
                    pos.X /= letDim;
                    pos.Y = (int)pos.Y / letDim;
                    int rowCount = textBoxEditor.Width / letDim;
                    cursor = (int)(pos.X + pos.Y * rowCount) + 1;
                    if(cursor >= letters.Count) {
                        cursor = letters.Count;
                    }
                }
            }

            // Change selected trait on click
            if(Mouse.GetState().LeftButton == ButtonState.Pressed) {
                Trait[] types = (Trait[])Enum.GetValues(typeof(Trait));
                foreach(Trait option in types) {
                    if((option == Trait.Attack && card.CardType != Card.Type.Monster) ||
                        (option == Trait.Cost && card.CardType == Card.Type.Fortress) ||
                        (option == Trait.Health && (card.CardType == Card.Type.Effect || card.CardType == Card.Type.Environment))
                    ) {
                        // ignore traits that don't exist for this card type
                        continue;
                    }

                    if(hitBoxes[option].Contains(Mouse.GetState().Position)) {
                        if(selected == Trait.Text) {
                            // update card text box to match edited version
                            String mended = "";
                            foreach(String letter in letters) {
                                mended += letter;
                            }
                            card.Text = mended;
                        }
                        selected = option;
                    }
                }
            }
        }

        public static void Draw(SpriteBatch sb) {
            card.Draw(sb, new Rectangle(500, 20, 600, 0));

            // draw selected option
            if(hitBoxes != null) {
                sb.Draw(Game1.Pixel, hitBoxes[selected], Color.LightBlue * 0.5f);
            }
            
            // draw text box editor
            Rectangle bordered = textBoxEditor;
            bordered.Inflate(10, 10);
            sb.Draw(Game1.Pixel, bordered, Color.White);
            int rowCount = textBoxEditor.Width / letDim;
            Vector2 topLeftMid = new Vector2(textBoxEditor.X + letDim / 2, textBoxEditor.Y + letDim / 2); // the middle of the top left square
            for(int i = 0; i < letters.Count; i++) {
                Vector2 dims = Game1.Font.MeasureString(letters[i]);
                sb.DrawString(Game1.Font, letters[i], topLeftMid - dims / 2 + letDim * new Vector2(i % rowCount, i / rowCount), Color.Black);
            }

            sb.Draw(Game1.Pixel, new Rectangle(textBoxEditor.X + cursor % rowCount * letDim - 2, textBoxEditor.Y + cursor / rowCount * letDim, 3, 20), Color.Blue);
        }

        public static void Save() {
            if(File.Exists("Content\\Cards\\" + card.FileName + ".card")) {
                Gallery.Remove(card.FileName);
            }
            card.Save(); // middle because it updates file name
            Gallery.Add(card.Name);
        }

        public static void Clear() {
            card.Text = "";
            letters.Clear();
            cursor = 0;
        }

        public static void Delete() {
            Gallery.Remove(card.FileName);
            card.Delete();
        }
    }
}
