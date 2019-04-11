using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files | *.xml|Xml files |*.xml*"; 
            dialog.Multiselect = false; 
            if (dialog.ShowDialog() == DialogResult.OK) 
            {
                String path = dialog.FileName;
                XmlDocument doc = new XmlDocument();
                int organisationnumber = 1;
                string currentorga = "";
                doc.Load(path);
                dynamic jsonbase = new JObject();
                dynamic organisationsliste = new JArray();
                dynamic items = new JObject();
                dynamic mounts = new JArray();
                int mountnumber = 1;
                dynamic organisations = new JArray();
                jsonbase.organisation = organisationsliste;
                jsonbase.magicitems = items;
                jsonbase.mounts = mounts;
                jsonbase.army = organisations;
                dynamic organisation = new JObject();
                Dictionary<string, dynamic> dicorganisations = new Dictionary<string, dynamic>();
                organisation.organisation = "1";
                organisations.Add(organisation);
                dynamic units = new JArray();
                organisation.units = units;
                foreach (XmlNode entry in doc.DocumentElement.ChildNodes)
                {
                    if (entry.Name == "selectionEntries")
                    {
                        foreach (XmlNode model in entry.ChildNodes)
                        {
                            if (model.Name == "selectionEntry")
                            {
                                dynamic profilerules = new JArray();
                                dynamic options = new JArray();
                                dynamic profilerule = new JObject();
                                profilerules.Add(profilerule);
                                dynamic unit = new JObject();
                                unit.name = model.Attributes["name"].Value;
                                unit.Base = "1";
                                dynamic specialrules = new JArray();
                                dynamic equipments = new JArray();
                                profilerule.specialrules = specialrules;
                                profilerule.equipments = equipments;
                                int cost = 0;
                                foreach (XmlNode profiles in model.ChildNodes)
                                {
                                    if (profiles.Name == "profiles" && profiles.ChildNodes.Count > 2)
                                    {
                                        bool firstOff = true;
                                        foreach (XmlNode profile in profiles.ChildNodes)
                                        {
                                            var charac = profile.ChildNodes[4];
                                            if (profile.Attributes["profileTypeName"].Value.Contains("Global"))
                                            {
                                                foreach (XmlNode characteristics in charac.ChildNodes)
                                                {
                                                    if (characteristics.Name == "characteristic")
                                                    {
                                                        switch (characteristics.Attributes["name"].Value)
                                                        {
                                                            case "Adv": profilerule.ADV = characteristics.Attributes["value"].Value.Replace("\"", ""); break;
                                                            case "Mar": profilerule.MAR = characteristics.Attributes["value"].Value.Replace("\"", ""); break;
                                                            case "Dis": profilerule.Ld = characteristics.Attributes["value"].Value; break;
                                                            case "Size": unit.size = characteristics.Attributes["value"].Value; break;
                                                            case "Type": unit.type = characteristics.Attributes["value"].Value; break;
                                                        }
                                                    }
                                                }
                                            }
                                            charac = profile.ChildNodes[4];
                                            if (profile.Attributes["profileTypeName"].Value.Contains("Defensive"))
                                            {
                                                foreach (XmlNode characteristics in charac.ChildNodes)
                                                {
                                                    if (characteristics.Name == "characteristic")
                                                    {
                                                        switch (characteristics.Attributes["name"].Value)
                                                        {
                                                            case "HP": profilerule.W = characteristics.Attributes["value"].Value; break;
                                                            case "Def": profilerule.DEF = characteristics.Attributes["value"].Value; break;
                                                            case "Res": profilerule.T = characteristics.Attributes["value"].Value; break;
                                                            case "Arm": profilerule.ARM = characteristics.Attributes["value"].Value; break;
                                                        }
                                                    }
                                                }
                                            }
                                            charac = profile.ChildNodes[4];
                                            if (profile.Attributes["profileTypeName"].Value.Contains("Offensive"))
                                            {
                                                if (firstOff)
                                                {
                                                    profilerule.name = profile.Attributes["name"].Value.Replace("Offence", "");
                                                    firstOff = false;
                                                }
                                                else
                                                {
                                                    profilerule = new JObject();
                                                    profilerules.Add(profilerule);
                                                    profilerule.name = profile.Attributes["name"].Value.Replace("Offence", "");
                                                    profilerule.ADV = "-";
                                                    profilerule.MAR = "-";
                                                    profilerule.W = "-";
                                                    profilerule.DEF = "-";
                                                    profilerule.T = "-";
                                                    profilerule.ARM = "-";
                                                    profilerule.Dis = "-";
                                                    dynamic specialrules2 = new JArray();
                                                    profilerule.specialrules = specialrules2;
                                                    dynamic equipments2 = new JArray();
                                                    profilerule.equipments = specialrules2;
                                                }
                                                foreach (XmlNode characteristics in charac.ChildNodes)
                                                {
                                                    if (characteristics.Name == "characteristic")
                                                    {
                                                        switch (characteristics.Attributes["name"].Value)
                                                        {
                                                            case "Att": profilerule.A = characteristics.Attributes["value"].Value; break;
                                                            case "Off": profilerule.OFF = characteristics.Attributes["value"].Value; break;
                                                            case "Str": profilerule.S = characteristics.Attributes["value"].Value; break;
                                                            case "AP": profilerule.AP = characteristics.Attributes["value"].Value; break;
                                                            case "Agi": profilerule.I = characteristics.Attributes["value"].Value; break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (profiles.Name == "categoryLinks")
                                    {
                                        foreach (XmlNode info in profiles.ChildNodes)
                                        {
                                            if (info.Name == "categoryLink" && info.Attributes["primary"].Value == "true" && currentorga != info.Attributes["targetId"].Value)
                                            {
                                                if (!string.IsNullOrEmpty(currentorga) && !dicorganisations.ContainsKey(info.Attributes["targetId"].Value))
                                                {
                                                    organisationnumber++;
                                                    organisation = new JObject();
                                                    organisation.organisation = organisationnumber.ToString();
                                                    organisations.Add(organisation);
                                                    units = new JArray();
                                                    organisation.units = units;
                                                    dicorganisations.Add(info.Attributes["targetId"].Value, organisation);
                                                }
                                                else if (!string.IsNullOrEmpty(currentorga) && dicorganisations.ContainsKey(info.Attributes["targetId"].Value))
                                                {
                                                    organisation = dicorganisations[info.Attributes["targetId"].Value];
                                                    units = organisation.units;
                                                }
                                                currentorga = info.Attributes["targetId"].Value;
                                            }
                                        }
                                    }
                                    else if (profiles.Name == "rules")
                                    {
                                        foreach (XmlNode info in profiles.ChildNodes)
                                        {
                                            if (info.Name == "rule")
                                            {
                                                dynamic specialrule = new JObject();
                                                specialrule.specialrule = info.Attributes["name"].Value;
                                                specialrules.Add(specialrule);
                                            }
                                        }
                                    }
                                    else if (profiles.Name == "infoLinks")
                                    {
                                        foreach (XmlNode info in profiles.ChildNodes)
                                        {
                                            if (info.Name == "infoLink")
                                            {
                                                if (info.Attributes["type"].Value == "rule")
                                                {
                                                    dynamic specialrule = new JObject();
                                                    var ruletext = info.Attributes["name"].Value;
                                                    specialrules.Add(specialrule);
                                                    if (info.ChildNodes[3].ChildNodes.Count > 0)
                                                    {
                                                        foreach (XmlNode modifier in info.ChildNodes[3].ChildNodes)
                                                        {
                                                            if (modifier.Attributes["type"].Value == "append" && modifier.Attributes["field"].Value == "name")
                                                            ruletext += " " + modifier.Attributes["value"].Value;
                                                        }
                                                    }
                                                    specialrule.specialrule = ruletext;
                                                }
                                                else
                                                {
                                                    dynamic equipment = new JObject();
                                                    equipment.equipment = info.Attributes["name"].Value;
                                                    equipments.Add(equipment);
                                                }
                                            }
                                        }
                                    }
                                    else if (profiles.Name == "entryLinks")
                                    {
                                        foreach (XmlNode info in profiles.ChildNodes)
                                        {
                                            if (info.Name == "entryLink" && info.Attributes["name"].Value == "Command Group")
                                            {
                                                dynamic option = new JObject();
                                                option.name = "Command";
                                                option.onechoice = false;
                                                dynamic optionchoices = new JArray();
                                                option.choices = optionchoices;
                                                dynamic optionchoice = new JObject();
                                                optionchoice.name = "Champion";
                                                optionchoice.cost = "20";
                                                optionchoices.Add(optionchoice);
                                                optionchoice = new JObject();
                                                optionchoice.name = "Musician";
                                                optionchoice.cost = "20";
                                                optionchoices.Add(optionchoice);
                                                optionchoice = new JObject();
                                                optionchoice.name = "Standard Bearer";
                                                optionchoice.cost = "20";
                                                optionchoices.Add(optionchoice);
                                                options.Add(option);
                                            }
                                        }
                                    }
                                    else if (profiles.Name == "costs")
                                    {
                                        foreach (XmlNode info in profiles.ChildNodes)
                                        {
                                            if (info.Name == "cost")
                                            {
                                                if (cost > 0)
                                                {
                                                    unit.cost = (int.Parse(info.Attributes["value"].Value.Replace(".0", "")) + cost).ToString();
                                                }
                                                else
                                                    unit.cost = info.Attributes["value"].Value.Replace(".0", "");
                                            }
                                        }
                                    }
                                    else if (profiles.Name == "selectionEntries")
                                    {
                                        dynamic option = new JObject();
                                        option.name = "Options";
                                        option.onechoice = false;
                                        dynamic optionchoices = new JArray();
                                        option.choices = optionchoices;
                                        options.Add(option);
                                        bool permodelstate = false;
                                        foreach (XmlNode info in profiles.ChildNodes)
                                        {
                                            if (info.Name == "selectionEntry" && info.Attributes["type"].Value == "upgrade")
                                            {
                                                bool permodel = false;
                                                dynamic optionchoice = new JObject();
                                                optionchoice.name = info.Attributes["name"].Value;
                                                foreach (XmlNode optioninfo in info.ChildNodes)
                                                {
                                                    foreach (XmlNode optioninfo2 in optioninfo.ChildNodes)
                                                    {
                                                        if (optioninfo2.Name == "cost" && !permodel)
                                                        {
                                                            optionchoice.cost = optioninfo2.Attributes["value"] != null ? optioninfo2.Attributes["value"].Value.Replace(".0", "") : "";
                                                        }
                                                        if (optioninfo2.Name == "modifier" && optioninfo2.Attributes["type"].Value == "increment")
                                                        {
                                                            permodel = true;
                                                            if (permodelstate != permodel)
                                                            {
                                                                option = new JObject();
                                                                option.name = info.Attributes["name"].Value;
                                                                option.onechoice = false;
                                                                optionchoices = new JArray();
                                                                option.choices = optionchoices;
                                                                options.Add(option);
                                                                permodelstate = permodel;
                                                            }
                                                            option.type = "permodel";
                                                            optionchoice.cost = optioninfo2.Attributes["value"] != null ? optioninfo2.Attributes["value"].Value.Replace(".0", "") : "";
                                                        }
                                                    }
                                                }
                                                optionchoices.Add(optionchoice);
                                            }
                                            else if (info.Name == "selectionEntry" && info.Attributes["type"].Value == "model")
                                            {
                                                int intmin = 0, intmax = 0;
                                                foreach (XmlNode nodeEntrys in info.ChildNodes)
                                                {
                                                    if (nodeEntrys.Name == "constraints")
                                                    {
                                                        foreach (XmlNode nodeEntry in nodeEntrys.ChildNodes)
                                                        {
                                                            if (nodeEntry.Name == "constraint")
                                                            {
                                                                if (nodeEntry.Attributes["type"].Value == "min")
                                                                {
                                                                    unit.unitsize = nodeEntry.Attributes["value"].Value.Replace(".0", "");
                                                                    intmin = int.Parse(nodeEntry.Attributes["value"].Value.Replace(".0", ""));
                                                                }
                                                                else if (nodeEntry.Attributes["type"].Value == "max")
                                                                {
                                                                    intmax = int.Parse(nodeEntry.Attributes["value"].Value.Replace(".0", ""));
                                                                }
                                                            }
                                                        }
                                                        if (intmin > 0 && intmax > 0)
                                                        {
                                                            unit.maxmodels = (intmax - intmin).ToString();
                                                        }
                                                    }
                                                    if (nodeEntrys.Name == "costs")
                                                    {
                                                        unit.modelcost = nodeEntrys.ChildNodes[0].Attributes["value"].Value.Replace(".0", "");
                                                        cost = int.Parse(nodeEntrys.ChildNodes[0].Attributes["value"].Value.Replace(".0", "")) * intmin;
                                                    }
                                                }
                                                bool firstOff = true;
                                                if (info.ChildNodes[0].ChildNodes.Count > 0)
                                                {
                                                    foreach (XmlNode profile in info.ChildNodes[0].ChildNodes)
                                                    {
                                                        var charac = profile.ChildNodes[4];
                                                        if (profile.Attributes["profileTypeName"].Value.Contains("Global"))
                                                        {
                                                            foreach (XmlNode characteristics in charac.ChildNodes)
                                                            {
                                                                if (characteristics.Name == "characteristic")
                                                                {
                                                                    switch (characteristics.Attributes["name"].Value)
                                                                    {
                                                                        case "Adv": profilerule.ADV = characteristics.Attributes["value"].Value.Replace("\"", ""); break;
                                                                        case "Mar": profilerule.MAR = characteristics.Attributes["value"].Value.Replace("\"", ""); break;
                                                                        case "Dis": profilerule.Ld = characteristics.Attributes["value"].Value; break;
                                                                        case "Size": unit.size = characteristics.Attributes["value"].Value; break;
                                                                        case "Type": unit.type = characteristics.Attributes["value"].Value; break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        charac = profile.ChildNodes[4];
                                                        if (profile.Attributes["profileTypeName"].Value.Contains("Defensive"))
                                                        {
                                                            foreach (XmlNode characteristics in charac.ChildNodes)
                                                            {
                                                                if (characteristics.Name == "characteristic")
                                                                {
                                                                    switch (characteristics.Attributes["name"].Value)
                                                                    {
                                                                        case "HP": profilerule.W = characteristics.Attributes["value"].Value; break;
                                                                        case "Def": profilerule.DEF = characteristics.Attributes["value"].Value; break;
                                                                        case "Res": profilerule.T = characteristics.Attributes["value"].Value; break;
                                                                        case "Arm": profilerule.ARM = characteristics.Attributes["value"].Value; break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        charac = profile.ChildNodes[4];
                                                        if (profile.Attributes["profileTypeName"].Value.Contains("Offensive"))
                                                        {
                                                            if (firstOff)
                                                            {
                                                                profilerule.name = profile.Attributes["name"].Value.Replace("Offence", "");
                                                                firstOff = false;
                                                            }
                                                            else
                                                            {

                                                                profilerule = new JObject();
                                                                profilerules.Add(profilerule);
                                                                profilerule.name = profile.Attributes["name"].Value.Replace("Offence", "");
                                                                profilerule.ADV = "-";
                                                                profilerule.MAR = "-";
                                                                profilerule.W = "-";
                                                                profilerule.DEF = "-";
                                                                profilerule.T = "-";
                                                                profilerule.ARM = "-";
                                                                profilerule.Dis = "-";
                                                                dynamic specialrules2 = new JArray();
                                                                profilerule.specialrules = specialrules2;
                                                                dynamic equipments2 = new JArray();
                                                                profilerule.equipments = specialrules2;
                                                            }
                                                            foreach (XmlNode characteristics in charac.ChildNodes)
                                                            {
                                                                if (characteristics.Name == "characteristic")
                                                                {
                                                                    switch (characteristics.Attributes["name"].Value)
                                                                    {
                                                                        case "Att": profilerule.A = characteristics.Attributes["value"].Value; break;
                                                                        case "Off": profilerule.OFF = characteristics.Attributes["value"].Value; break;
                                                                        case "Str": profilerule.S = characteristics.Attributes["value"].Value; break;
                                                                        case "AP": profilerule.AP = characteristics.Attributes["value"].Value; break;
                                                                        case "Agi": profilerule.I = characteristics.Attributes["value"].Value; break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (profiles.Name == "selectionEntryGroups")
                                    {
                                        foreach (XmlNode info in profiles.ChildNodes)
                                        {
                                            if (info.Name == "selectionEntryGroup")
                                            {
                                                bool mandatory = false;
                                                bool multiple = false;
                                                foreach (XmlNode info0 in info.ChildNodes)
                                                {
                                                    if (info0.Name == "constraints" && info0.ChildNodes.Count > 0)
                                                    {
                                                        foreach (XmlNode info2 in info0.ChildNodes)
                                                        {
                                                            if (info2.Attributes["type"].Value == "min")
                                                            {
                                                                var min = int.Parse(info2.Attributes["value"].Value.Replace(".0", ""));
                                                                if (min > 1)
                                                                {
                                                                    multiple = true;
                                                                    mandatory = true;
                                                                }
                                                                else if (min == 1)
                                                                    mandatory = true;
                                                                
                                                            }
                                                            else if (info2.Attributes["type"].Value == "max")
                                                            {
                                                                var max = int.Parse(info2.Attributes["value"].Value.Replace(".0", ""));
                                                                if (max > 1)
                                                                {
                                                                    multiple = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (info0.Name == "selectionEntries")
                                                    {
                                                        dynamic option = new JObject();
                                                        option.name = info.Attributes["name"].Value;
                                                        option.onechoice = !multiple;
                                                        option.mandatory = mandatory;
                                                        dynamic optionchoices = new JArray();
                                                        option.choices = optionchoices;
                                                        options.Add(option);
                                                        bool permodel = false;
                                                        foreach (XmlNode info2 in info0.ChildNodes)
                                                        {
                                                            dynamic optionchoice = new JObject();
                                                            optionchoices.Add(optionchoice);
                                                            if (info2.Name == "selectionEntry")
                                                            {
                                                                optionchoice.name = info2.Attributes["name"].Value;
                                                                foreach (XmlNode optioninfo in info2.ChildNodes)
                                                                {
                                                                    foreach (XmlNode optioninfo2 in optioninfo.ChildNodes)
                                                                    {
                                                                        if (optioninfo2.Name == "cost" && !permodel)
                                                                        {
                                                                            optionchoice.cost = optioninfo2.Attributes["value"] != null ? optioninfo2.Attributes["value"].Value.Replace(".0", "") : "";
                                                                        }
                                                                        if (optioninfo2.Name == "modifier" && optioninfo2.Attributes["type"].Value == "increment")
                                                                        {
                                                                            option.type = "permodel";
                                                                            permodel = true;
                                                                            optionchoice.cost = optioninfo2.Attributes["value"] != null ? optioninfo2.Attributes["value"].Value.Replace(".0", "") : "";
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                unit.rules = profilerules;
                                unit.options = options;
                                units.Add(unit);
                            }
                        }
                    }
                    else if (entry.Name == "sharedSelectionEntryGroups")
                    {
                        int count = 0;
                        foreach (XmlNode model in entry.ChildNodes)
                        {
                            if (model.Name == "selectionEntryGroup")
                            {
                                dynamic itementry = new JArray();
                                if (model.Attributes["name"].Value.ToLower().Contains("weapon"))
                                    items.weapons = itementry;
                                else if (model.Attributes["name"].Value.ToLower().Contains("armour"))
                                    items.armours = itementry;
                                else if (model.Attributes["name"].Value.ToLower().Contains("artefact"))
                                    items.artefacts = itementry;
                                else if (model.Attributes["name"].Value.ToLower().Contains("banner"))
                                    items.standards = itementry;
                                else
                                {
                                    switch (count)
                                    {
                                        case 0: items.category1 = itementry;break;
                                        case 1: items.category2 = itementry; break;
                                        case 2: items.category3 = itementry; break;
                                        case 3: items.category4 = itementry; break;
                                        case 4: items.category5 = itementry; break;
                                        case 5: items.category6 = itementry; break;
                                    }
                                    count++;
                                }
                                foreach (XmlNode selectionentries in model.ChildNodes)
                                {
                                    if (selectionentries.Name == "selectionEntries")
                                    {
                                        foreach (XmlNode selectionentry in selectionentries.ChildNodes)
                                        {
                                            dynamic itemrule = new JObject();
                                            itemrule.name = selectionentry.Attributes["name"].Value;
                                            if (selectionentry.Name == "selectionEntry")
                                            {

                                                foreach (XmlNode profiles in selectionentry.ChildNodes)
                                                {
                                                    if (profiles.Name == "profiles" && profiles.ChildNodes.Count > 0)
                                                    {
                                                        foreach (XmlNode profile in profiles.ChildNodes[0].ChildNodes)
                                                        {
                                                            var description = "";
                                                            if (profile.Name == "characteristics")
                                                            {
                                                                foreach (XmlNode charac in profile.ChildNodes)
                                                                {
                                                                    if (charac.Name == "characteristic")
                                                                    {
                                                                        description += charac.Attributes["value"].Value;
                                                                    }
                                                                }
                                                            }
                                                            itemrule.description = description;
                                                        }
                                                    }
                                                    else if (profiles.Name == "costs")
                                                    {
                                                        itemrule.cost = profiles.ChildNodes[0].Attributes["value"].Value.Replace(".0", "");
                                                    }
                                                }
                                                itementry.Add(itemrule);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (entry.Name == "sharedSelectionEntries")
                    {
                        foreach (XmlNode selectionentry in entry.ChildNodes)
                        {
                            if (selectionentry.Name == "selectionEntry")
                            {
                                dynamic specialrules = new JArray();
                                dynamic equipments = new JArray();
                                foreach (XmlNode profile in selectionentry.ChildNodes)
                                {
                                    if (profile.Name == "infoLinks")
                                    {
                                        foreach (XmlNode info in profile.ChildNodes)
                                        {
                                            if (info.Name == "infoLink")
                                            {
                                                if (info.Attributes["type"].Value == "rule")
                                                {
                                                    dynamic specialrule = new JObject();
                                                    var ruletext = info.Attributes["name"].Value;
                                                    specialrules.Add(specialrule);
                                                    if (info.ChildNodes[3].ChildNodes.Count > 0)
                                                    {
                                                        foreach (XmlNode modifier in info.ChildNodes[3].ChildNodes)
                                                        {
                                                            if (modifier.Attributes["type"].Value == "append" && modifier.Attributes["field"].Value == "name")
                                                                ruletext += " " + modifier.Attributes["value"].Value;
                                                        }
                                                    }
                                                    specialrule.specialrule = ruletext;
                                                }
                                                else
                                                {
                                                    dynamic equipment = new JObject();
                                                    equipment.equipment = info.Attributes["name"].Value;
                                                    equipments.Add(equipment);
                                                }
                                            }
                                        }
                                    }
                                    if (profile.Name == "profiles" && profile.ChildNodes.Count > 0)
                                    {
                                        dynamic mount = new JObject();
                                        mounts.Add(mount);
                                        mount.id = mountnumber.ToString();
                                        mount.name = selectionentry.Attributes["name"].Value;
                                        dynamic mountrules = new JArray();
                                        mount.rules = mountrules;
                                        dynamic mountrule = new JObject();
                                        mountrules.Add(mountrule);
                                        mountrule.specialrules = specialrules;
                                        mountrule.equipments = equipments;
                                        mountnumber++;
                                        bool firstOff = true;
                                        foreach (XmlNode profiler in profile.ChildNodes)
                                        {
                                            var charac = profiler.ChildNodes[4];
                                            if (profiler.Attributes["profileTypeName"].Value.Contains("Global"))
                                            {
                                                foreach (XmlNode characteristics in charac.ChildNodes)
                                                {
                                                    if (characteristics.Name == "characteristic")
                                                    {
                                                        switch (characteristics.Attributes["name"].Value)
                                                        {
                                                            case "Adv": mountrule.ADV = characteristics.Attributes["value"].Value.Replace("\"", ""); break;
                                                            case "Mar": mountrule.MAR = characteristics.Attributes["value"].Value.Replace("\"", ""); break;
                                                            case "Dis": mountrule.Ld = characteristics.Attributes["value"].Value; break;
                                                            case "Size": mount.size = characteristics.Attributes["value"].Value; break;
                                                            case "Type": mount.type = characteristics.Attributes["value"].Value; break;
                                                        }
                                                    }
                                                }
                                            }
                                            charac = profiler.ChildNodes[4];
                                            if (profiler.Attributes["profileTypeName"].Value.Contains("Defensive"))
                                            {
                                                foreach (XmlNode characteristics in charac.ChildNodes)
                                                {
                                                    if (characteristics.Name == "characteristic")
                                                    {
                                                        switch (characteristics.Attributes["name"].Value)
                                                        {
                                                            case "HP": mountrule.W = characteristics.Attributes["value"].Value; break;
                                                            case "Def": mountrule.DEF = characteristics.Attributes["value"].Value; break;
                                                            case "Res": mountrule.T = characteristics.Attributes["value"].Value; break;
                                                            case "Arm": mountrule.ARM = characteristics.Attributes["value"].Value; break;
                                                        }
                                                    }
                                                }
                                            }
                                            charac = profiler.ChildNodes[4];
                                            if (profiler.Attributes["profileTypeName"].Value.Contains("Offensive"))
                                            {
                                                if (firstOff)
                                                {
                                                    mountrule.name = profiler.Attributes["name"].Value.Replace("Offence", "");
                                                    firstOff = false;
                                                }
                                                else
                                                {

                                                    mountrule = new JObject();
                                                    mountrules.Add(mountrule);
                                                    mountrule.name = profiler.Attributes["name"].Value.Replace("Offence", "");
                                                    mountrule.ADV = "-";
                                                    mountrule.MAR = "-";
                                                    mountrule.W = "-";
                                                    mountrule.DEF = "-";
                                                    mountrule.T = "-";
                                                    mountrule.ARM = "-";
                                                    mountrule.Dis = "-";
                                                    dynamic specialrules2 = new JArray();
                                                    mountrule.specialrules = specialrules2;
                                                    dynamic equipments2 = new JArray();
                                                    mountrule.equipments = specialrules2;
                                                }
                                                foreach (XmlNode characteristics in charac.ChildNodes)
                                                {
                                                    if (characteristics.Name == "characteristic")
                                                    {
                                                        switch (characteristics.Attributes["name"].Value)
                                                        {
                                                            case "Att": mountrule.A = characteristics.Attributes["value"].Value; break;
                                                            case "Off": mountrule.OFF = characteristics.Attributes["value"].Value; break;
                                                            case "Str": mountrule.S = characteristics.Attributes["value"].Value; break;
                                                            case "AP": mountrule.AP = characteristics.Attributes["value"].Value; break;
                                                            case "Agi": mountrule.I = characteristics.Attributes["value"].Value; break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                int i = 1;
                foreach (var elt in dicorganisations)
                {
                    dynamic orga = new JObject();
                    orga.id = i.ToString();
                    orga.percent = 100;
                    orga.type = "max";
                    orga.name = i == 1 ? "Characters" : (i == 2 ? "Core" : (i == 3 ? "Special" : ""));
                    organisationsliste.Add(orga);
                    i++;
                }
                System.IO.File.WriteAllText(dialog.FileName.Replace(".txt", "").Replace(".xml", "") + "extract" + ".txt", jsonbase.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
