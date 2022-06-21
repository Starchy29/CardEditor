using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CardEditor
{
    class Menu
    {
        private List<Button> buttons;

        public Menu() { 
            this.buttons = new List<Button>();
        }

        public void Add(Button button) {
            buttons.Add(button);
        }

        public void Update() {
            foreach(Button button in buttons) {
                button.Update();
            }
        }

        public void Draw(SpriteBatch sb) {
            foreach(Button button in buttons) {
                button.Draw(sb);
            }
        }
    }
}
