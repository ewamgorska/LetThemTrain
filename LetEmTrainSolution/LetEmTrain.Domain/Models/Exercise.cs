using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetEmTrain.Domain.Models
{
    public class Exercise : Entity 
    {
        public string Name { get; set; }
        public string MainBodyPart { get; set; }   
        public byte[] Image {  get; set; }
        public string Description { get; set; }
        public List<ExerciseSet> ExerciseSets { get; set; }

        public Exercise()
        {
            ExerciseSets = new List<ExerciseSet>();
        }

    }
}
