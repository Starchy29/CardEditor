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
    class Card
    {
        public enum Type {
            Fortress,
            Structure,
            Monster,
            Effect,
            Environment
        }

        private Type type;
        private String name;
        private int cost;
        private List<String> text; // each element is a line of text

        private int health;
        private int attack;

        public Type CardType { get { return type; } }
        public String Name {
            get { return name; }
            set { name = value; }
        }
        public int Cost { set { cost = value; } }
        public int Health { set { health = value; } }
        public int Attack { set { attack = value; } }

        private String fileName; // track if this card was edited from a file

        // load from a file
        public Card(String name) {
            this.name = name;
            this.fileName = name;

            StreamReader input = null;
            try {
                input = new StreamReader("Content\\Cards\\" + name + ".card");

                String[] data = input.ReadLine().Split(',');

                type = (Type)Enum.Parse(typeof(Type), data[0]);
                cost = int.Parse(data[1]);
                health = int.Parse(data[2]);
                attack = int.Parse(data[3]);

                text = new List<String>();
                String currentLine = null;
                while((currentLine = input.ReadLine()) != null) {
                    text.Add(currentLine);
                }

            }
            catch(Exception e) {
                // placeholder card so no crash
                this.type = Type.Fortress;

                name = "Missing Card";
                text = new List<string>();
                text.Add("Encountered an error.");
                cost = 0;
                health = 0;
                attack = 0;
            }
            finally {
                if(input != null) {
                    input.Close();
                }
            }
        }

        // create a new blank card
        public Card(Type type) {
            this.type = type;

            name = "New Card";
            text = new List<String>();
            cost = 0;
            health = 0;
            attack = 0;

            fileName = null;
        }

        // area.Height is ignored, height is proportional to the input width
        public void Draw(SpriteBatch sb, Rectangle area) {
            area.Height = area.Width * Game1.Fortress.Height / Game1.Fortress.Width;

            // draw background
            Texture2D background = null;
            switch(type) {
                case Type.Effect:
                    background = Game1.Effect;
                    break;

                case Type.Fortress:
                    background = Game1.Fortress;
                    break;

                case Type.Monster:
                    background = Game1.Monster;
                    break;

                case Type.Structure:
                    background = Game1.Structure;
                    break;

                case Type.Environment:
                    background = Game1.Environment;
                    break;
            }

            sb.Draw(background, area, Color.White);

            // draw name and stats
            float scale = (float)area.Width / Game1.Fortress.Width;

            void DrawCentered(String text, Vector2 center) { // center: (0, 0) is top-left and (1, 1) is bottom-right
                Vector2 dims = Game1.Font.MeasureString(text) * scale;
                Vector2 mid = new Vector2(area.X + area.Width * center.X, area.Y + area.Height * center.Y);
                sb.DrawString(Game1.Font, text, mid - dims / 2, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            DrawCentered(name, new Vector2(0.5f, 0.1f));
            

            if(type != Type.Fortress) {
                DrawCentered("" + cost, new Vector2(0.11f, 0.93f));
            }
            if(type != Type.Effect && type != Type.Environment) {
                DrawCentered("" + health, new Vector2(0.9f, 0.93f));
            }
            if(type == Type.Monster) {
                DrawCentered("" + attack, new Vector2(0.67f, 0.93f));
            }

            // draw text box
            float lineWidth = 0.05f;
            scale *= 0.9f; // shrink rules text
            for(int i = 0; i < text.Count; i++) {
                DrawCentered(text[i], new Vector2(0.5f, 0.61f + lineWidth * i));
            }
        }

        public void Save() {
            StreamWriter output = null; 
            try {
                output = new StreamWriter("Content\\Cards\\" + name + ".card");

                output.Write(Enum.GetName(typeof(Type), type) + ",");
                output.Write(cost + ",");
                output.Write(health + ",");
                output.Write(attack + "\n");

                output.Write(text);

                // delete old card if not overwritten
                if (fileName != name) {
                    File.Delete("Content\\Cards\\" + fileName + ".card");
                    fileName = name;
                }
            }
            catch(Exception e) {
                // report that save failed?
            }
            finally {
                output.Close();
            }
        }
    }
}
