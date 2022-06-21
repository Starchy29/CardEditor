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
    public delegate void ClickEvent();

    public class Button
    {
        private Rectangle area;
        private ClickEvent OnClick;
        private String text;

        private bool hovered;

        public Button(Rectangle area, String text, ClickEvent click) {
            this.area = area;
            this.text = text;
            OnClick = click;
        }

        public void Update() {
            hovered = area.Contains(Mouse.GetState().Position);

            if(hovered && Game1.JustClicked()) {
                OnClick();
            }
        }

        public void Draw(SpriteBatch sb) {
            sb.Draw(Game1.Pixel, area, hovered ? Color.Blue : Color.White);
            Rectangle inner = area;
            inner.Inflate(-10, -10);
            sb.Draw(Game1.Pixel, inner, Color.Black);

            Vector2 dims = Game1.Font.MeasureString(text);
            sb.DrawString(Game1.Font, text, new Vector2(area.X + (area.Width - dims.X) / 2, area.Y + (area.Height - dims.Y) / 2), Color.White);
        }
    }
}
