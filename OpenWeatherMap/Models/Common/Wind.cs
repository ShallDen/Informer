// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Wind.cs" company="Joan Caron">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Joan Caron</author>
// <summary>Implements the wind class</summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenWeatherMap
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using System.Data.Entity;

    /// <summary>
    ///     Class Wind.
    /// </summary>
    public class Wind
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        ///     Gets or sets the speed.
        /// </summary>
        /// <value>
        ///     The speed.
        /// </value>
        [XmlElement("speed")]
        public Speed Speed { get; set; }

        /// <summary>
        ///     Gets or sets the direction.
        /// </summary>
        /// <value>
        ///     The direction.
        /// </value>
        [XmlElement("direction")]
        public Direction Direction { get; set; }
    }
}
