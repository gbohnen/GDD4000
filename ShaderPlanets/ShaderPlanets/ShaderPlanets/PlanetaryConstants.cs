using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderPlanets
{
    public struct PlanetData
    {
        public float Period;
        public float AngularPeriod;
        public float Diameter;
        public float OrbitalRadius;
    }

    static class PlanetaryConstants
    {
        // game constants
        public const float CAMERA_DISTANCE = 1500;

        // sol
        public const float SOL_PERIOD = 1f;
        public const float SOL_ANG_PER = 1f;
        public const float SOL_DIAMETER = 3f;
        public const float SOL_ORBITAL_RADIUS = 0f;

        // mercury
        public const float MERCURY_PERIOD = 1f;
        public const float MERCURY_ANG_PER = 1f;
        public const float MERCURY_DIAMETER = .5f;
        public const float MERCURY_ORBITAL_RADIUS = 200f;

        // venus
        public const float VENUS_PERIOD = 1f;
        public const float VENUS_ANG_PER = 1f;
        public const float VENUS_DIAMETER = .5f;
        public const float VENUS_ORBITAL_RADIUS = 50f;

        // earth
        public const float EARTH_PERIOD = 1f;
        public const float EARTH_ANG_PER = 1f;
        public const float EARTH_DIAMETER = 1f;
        public const float EARTH_ORBITAL_RADIUS = 3f;

        // luna
        public const float LUNA_PERIOD = 1f;
        public const float LUNA_ANG_PER = 1f;
        public const float LUNA_DIAMETER = 1f;
        public const float LUNA_ORBITAL_RADIUS = -1f;

        // mars
        public const float MARS_PERIOD = 1f;
        public const float MARS_ANG_PER = 1f;
        public const float MARS_DIAMETER = 1f;
        public const float MARS_ORBITAL_RADIUS = -2f;

        // jupiter
        public const float JUPITER_PERIOD = 1f;
        public const float JUPITER_ANG_PER = 1f;
        public const float JUPITER_DIAMETER = 2f;
        public const float JUPITER_ORBITAL_RADIUS = -3f;

        // saturn
        public const float SATURN_PERIOD = 1f;
        public const float SATURN_ANG_PER = 1f;
        public const float SATURN_DIAMETER = 2f;
        public const float SATURN_ORBITAL_RADIUS = 0f;

        // uranus
        public const float URANUS_PERIOD = 1f;
        public const float URANUS_ANG_PER = 1f;
        public const float URANUS_DIAMETER = 2f;
        public const float URANUS_ORBITAL_RADIUS = 0f;

        // neptune
        public const float NEPTUNE_PERIOD = 1f;
        public const float NEPTUNE_ANG_PER = 1f;
        public const float NEPTUNE_DIAMETER = 2f;
        public const float NEPTUNE_ORBITAL_RADIUS = 0f;

        // pluto
        public const float PLUTO_PERIOD = 1f;
        public const float PLUTO_ANG_PER = 1f;
        public const float PLUTO_DIAMETER = .5f;
        public const float PLUTO_ORBITAL_RADIUS = 0f;

        public static PlanetData GetPlanet(Planets planet)
        {
            PlanetData data;

            switch (planet)
            {
                case Planets.Sol:
                    data.Period =           SOL_PERIOD;
                    data.AngularPeriod =    SOL_ANG_PER;
                    data.Diameter =         SOL_DIAMETER;
                    data.OrbitalRadius =    SOL_ORBITAL_RADIUS;
                    break;
                case Planets.Mercury:
                    data.Period = MERCURY_PERIOD;
                    data.AngularPeriod = MERCURY_ANG_PER;
                    data.Diameter = MERCURY_DIAMETER;
                    data.OrbitalRadius = MERCURY_ORBITAL_RADIUS;
                    break;
                case Planets.Venus:
                    data.Period = VENUS_PERIOD;
                    data.AngularPeriod = VENUS_ANG_PER;
                    data.Diameter = VENUS_DIAMETER;
                    data.OrbitalRadius = VENUS_ORBITAL_RADIUS;
                    break;
                case Planets.Earth:
                    data.Period = EARTH_PERIOD;
                    data.AngularPeriod = EARTH_ANG_PER;
                    data.Diameter = EARTH_DIAMETER;
                    data.OrbitalRadius = EARTH_ORBITAL_RADIUS;
                    break;
                case Planets.Luna:
                    data.Period = LUNA_PERIOD;
                    data.AngularPeriod = LUNA_ANG_PER;
                    data.Diameter = LUNA_DIAMETER;
                    data.OrbitalRadius = LUNA_ORBITAL_RADIUS;
                    break;
                case Planets.Mars:
                    data.Period = MARS_PERIOD;
                    data.AngularPeriod = MARS_ANG_PER;
                    data.Diameter = MARS_DIAMETER;
                    data.OrbitalRadius = MARS_ORBITAL_RADIUS;
                    break;
                case Planets.Jupiter:
                    data.Period = JUPITER_PERIOD;
                    data.AngularPeriod = JUPITER_ANG_PER;
                    data.Diameter = JUPITER_DIAMETER;
                    data.OrbitalRadius = JUPITER_ORBITAL_RADIUS;
                    break;
                case Planets.Saturn:
                    data.Period = SATURN_PERIOD;
                    data.AngularPeriod = SATURN_ANG_PER;
                    data.Diameter = SATURN_DIAMETER;
                    data.OrbitalRadius = SATURN_ORBITAL_RADIUS;
                    break;
                case Planets.Uranus:
                    data.Period = URANUS_PERIOD;
                    data.AngularPeriod = URANUS_ANG_PER;
                    data.Diameter = URANUS_DIAMETER;
                    data.OrbitalRadius = URANUS_ORBITAL_RADIUS;
                    break;
                case Planets.Neptune:
                    data.Period = NEPTUNE_PERIOD;
                    data.AngularPeriod = NEPTUNE_ANG_PER;
                    data.Diameter = NEPTUNE_DIAMETER;
                    data.OrbitalRadius = NEPTUNE_ORBITAL_RADIUS;
                    break;
                case Planets.Pluto:
                    data.Period = PLUTO_PERIOD;
                    data.AngularPeriod = PLUTO_ANG_PER;
                    data.Diameter = PLUTO_DIAMETER;
                    data.OrbitalRadius = PLUTO_ORBITAL_RADIUS;
                    break;
                default:
                    data.Period = 0f;
                    data.AngularPeriod = 0f;
                    data.Diameter = 0f;
                    data.OrbitalRadius = SOL_ORBITAL_RADIUS;
                    break;
            }

            return data;
        }
    }
}
