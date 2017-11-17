using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThumbnailCreatorWebJob.Listeners;

namespace ThumbnailCreatorWebJob
{
    class Program
    {

        static void Main(string[] args)
        {
            ThumbnailListener listener = new ThumbnailListener();
            listener.Listen();
        }
    }
}
