using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardEditor
{
    static class Text
    {
        private static Keys[] pressed;
        private static Keys[] lastPressed; // only one key per press

        public static void Update() {
            lastPressed = pressed;
            pressed = Keyboard.GetState().GetPressedKeys();
        }

        public static Keys GetCurrentKey() {
            foreach(Keys key in pressed) {
                if(!lastPressed.Contains(key)) {
                    return key;
                }
            }

            return Keys.None;
        }

        // converts the keyboard key to the proper letter
        public static String KeyToLetter(Keys key) {
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
            else if(key == Keys.OemSemicolon) {
                return ":";
            }
            else if(key == Keys.OemPlus) {
                return "+";
            }
            else if(key == Keys.OemMinus) {
                return "-";
            }
            else if(key == Keys.Enter) {
                return "%"; // percent represents a line break
            }
            else if(name.Length == 2 && name[0] == 'D') { // check for numbers
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
