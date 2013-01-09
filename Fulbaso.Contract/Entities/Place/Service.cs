using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Fulbaso.Contract
{
    public enum Service
    {
        [Description("Bar")]
        Eatery = 1,

        [Description("Vestuarios")]
        DressingRoom = 2,

        [Description("Estacionamiento")]
        Parking = 3,

        [Description("Ayuda Médica")]
        EmergencyMedicalAid = 4,

        [Description("Colegios")]
        School = 5,

        [Description("Torneos")]
        Tournament = 6,

        [Description("Parrillas")]
        Grill = 7,

        [Description("Cumpleaños")]
        Birthday = 8,

        [Description("Escuelita")]
        SoccerSchool = 9,

        [Description("Gimnasio")]
        Gym = 10,
    }
}
