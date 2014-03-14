using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flock.DTO;

namespace Flock.Models
{
    public class QuacksList
    {
        public IList<QuackDto> Quacks { get; set; }
        public int QuackCount { get; set; }
    }
}