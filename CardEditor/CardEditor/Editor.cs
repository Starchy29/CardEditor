using System;
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
        private static Keys[] lastPressed; // only one key per press

        public static void Load(String name) {
            card = new Card(name);
        }

        public static void New(Card.Type type) {
            card = new Card(type);
        }

        public static void Update() {
            if(hitBoxes == null) {
                hitBoxes = new Dictionary<Trait, Rectangle>();
                hitBoxes[Trait.Name] = new Rectangle(540, 60, 520, 80);
                hitBoxes[Trait.Cost] = new Rectangle(500, 800, 80, 80);
                hitBoxes[Trait.Attack] = new Rectangle();
                hitBoxes[Trait.Health] = new Rectangle();
                hitBoxes[Trait.Text] = new Rectangle();
            }

            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            foreach(Keys key in pressedKeys) {
                if(lastPressed.Contains(key)) {
                    continue; // one key register per press
                }

                switch(selected) {
                    case Trait.Name:
                        if(key == Keys.Back) {
                            String name = card.Name;
                            if(name.Length > 0) {
                                card.Name = name.Substring(0, name.Length - 1);
                            }
                        } else {
                            card.Name = card.Name + KeyToLetter(key); // add character to name
                        }
                        break;

                    case Trait.Attack:
                        break;

                    case Trait.Health:
                        break;

                    case Trait.Text:
                        break;

                    case Trait.Cost:
                        break;
                }
            }

            lastPressed = pressedKeys;

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
                        selected = option;
                    }
                }
            }
        }

        public static void Draw(SpriteBatch sb) {
            card.Draw(sb, new Rectangle(500, 20, 600, 0));

            // draw selected option
            sb.Draw(Game1.Pixel, hitBoxes[selected], Color.LightBlue * 0.5f);
        }

        // converts the keyboard key to the proper letter
        private static String KeyToLetter(Keys key) {
            String name = Enum.GetName(typeof(Keys), key);

            
            if(key == Keys.Space) {
                return " ";
            }
            else if(key == Keys.OemPeriod) {
                return ".";
            }
            else if(key == Keys.OemComma) {
                return ",";
            }
            else if(name.Length > 1 && name[0] == 'D') { // check for numbers
                return "" + name[1];
            }
            else if(name.Length == 1) { // check for regular letters
                if(Keyboard.GetState().IsKeyUp(Keys.RightShift)) { // lowercase if shift is not held
                    name = name.ToLower();
                }
                return name;
            }

            return ""; // do nothing if bad key is pressed
        }
    }
}
