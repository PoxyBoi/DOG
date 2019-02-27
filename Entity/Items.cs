﻿using System;
using System.Collections.Generic;

namespace DOG
{
    public partial class Items
    {
        public int Id { get; set; }
        public int Slot { get; set; }
        public int DogId { get; set; }
        public int AtkPower { get; set; }
        public int Defense { get; set; }
        public int Will { get; set; }
        public int Intelligence { get; set; }
        public int SpecialEffect { get; set; }
        public string Name { get; set; }

        public virtual Dogs Dog { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Slot: {Slot}, DogId: {DogId}, AtkPower: {AtkPower}, Defense: {Defense}, Will: {Will}, Intelligence: {Intelligence}, SpecialEffect: {SpecialEffect}, Name: {Name}, Dog: {Dog}";
        }
    }
}