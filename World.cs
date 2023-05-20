using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleMouseController
{
    internal class World
    {
        private DateTime updateEnd;
        private DateTime updateStart;

        private MouseController mouseController;

        public World()
        {
            mouseController = new MouseController();
        }
        
        public void Update()
        {
            while (true)
            {
                updateEnd = DateTime.Now;
                TimeSpan timeElapsed = updateEnd - updateStart;

                updateStart = DateTime.Now;

                mouseController.Update(timeElapsed);
            }
        }
    }
}
