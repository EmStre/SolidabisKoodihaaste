using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BullshitChallenge.Models
{
    // Lists of bullshit and not-bullshit to be sent to the view.
    public class BullshitViewModel
    {
        public List<string> Bullshit = new List<string>();

        public List<string> NotBullshit = new List<string>();

    }
}
