﻿using System.Configuration;

namespace Orders.com.Web.MVC.Configuration
{
    public class DIConfig : ConfigurationSection
    {
        [ConfigurationProperty("fromType", IsKey=true)]
        public string FromType
        {
            get { return (string)base["fromType"]; }
            set { base["fromType"] = value; }
        }

        [ConfigurationProperty("toType")]
        public string ToType
        {
            get { return (string)base["toType"]; }
            set { base["toType"] = value; }
        }

        [ConfigurationProperty("defaultProperties")]
        public DIDefaultPropConfigList DefaultProperties
        {
            get { return (DIDefaultPropConfigList)base["defaultProperties"]; }
            set { base["defaultProperties"] = value; }
        }
        
        [ConfigurationProperty("constructorArguments")]
        public DIConstructorArgConfigList ConstructorArguments
        {
            get { return (DIConstructorArgConfigList)base["constructorArguments"]; }
            set { base["constructorArguments"] = value; }
        }

        [ConfigurationProperty("asSingleton")]
        public bool AsSingleton
        {
            get { return (bool)base["asSingleton"]; }
            set { base["asSingleton"] = value; }
        }
    }
}
