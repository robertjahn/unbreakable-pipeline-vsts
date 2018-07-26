using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceSelfHealingFunction.VSTS.ApiObjects.Releases.List
{

    public class ListReleasesRootobject
    {
        public int count { get; set; }
        public Release[] value { get; set; }
    }

    public class Release
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public Modifiedby modifiedBy { get; set; }
        public Createdby createdBy { get; set; }
        public Variables variables { get; set; }
        public object[] variableGroups { get; set; }
        public Releasedefinition releaseDefinition { get; set; }
        public string description { get; set; }
        public string reason { get; set; }
        public string releaseNameFormat { get; set; }
        public bool keepForever { get; set; }
        public int definitionSnapshotRevision { get; set; }
        public string logsContainerUrl { get; set; }
        public string url { get; set; }
        public _Links3 _links { get; set; }
        public object[] tags { get; set; }
        public object triggeringArtifactAlias { get; set; }
        public Projectreference projectReference { get; set; }
        public Properties properties { get; set; }
    }

    public class Modifiedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links
    {
        public Avatar avatar { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class Createdby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links1 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links1
    {
        public Avatar1 avatar { get; set; }
    }

    public class Avatar1
    {
        public string href { get; set; }
    }

    public class Variables
    {
    }

    public class Releasedefinition
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public _Links2 _links { get; set; }
    }

    public class _Links2
    {
        public Self self { get; set; }
        public Web web { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }

    public class _Links3
    {
        public Self1 self { get; set; }
        public Web1 web { get; set; }
    }

    public class Self1
    {
        public string href { get; set; }
    }

    public class Web1
    {
        public string href { get; set; }
    }

    public class Projectreference
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Properties
    {
    }

}
