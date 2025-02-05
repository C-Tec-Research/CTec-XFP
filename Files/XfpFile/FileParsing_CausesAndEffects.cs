using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.Files.XfpFile
{
    internal partial class FileParsingXfp
    {
        private static void parseCETable(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                switch (ItemName(currentLine))
                {
                    case XfpTags.ArrayCEArray:    parseCEArray(inputStream, ref result);   break;
                    case XfpTags.ArrayTimeEvents: parseTimeArray(inputStream, ref result); break;
                }
            }
        }


        private static void parseCEArray(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
                if (ItemName(currentLine) == XfpTags.ArrayItem)
                    parseCEArrayItem(inputStream, ref result, parseItemIndex(currentLine));
        }


        private static void parseTimeArray(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            int i = 0;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
                if (ItemName(currentLine) == XfpTags.Item)
                    if (i++ >= CEEventListBase)
                        result.CurrentPanel.CEConfig.TimerEventTimes[parseItemIndex(currentLine) - 1] = parseTime(currentLine);
        }


        private static void parseCEArrayItem(StreamReader inputStream, ref XfpData result, int eventNum)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                if (eventNum >= CEEventListBase)
                {
                    if (ItemName(currentLine) == XfpTags.Item && parseItemIndex(currentLine) >= CEEventItemListBase)
                    {
                        switch (parseItemIndex(currentLine))
                        {
                            case 1: setActionType(result.CurrentPanel.CEConfig.Events[eventNum - 1],    parseInt(currentLine)); break;
                            case 2: setActionTarget(result.CurrentPanel.CEConfig.Events[eventNum - 1],  parseInt(currentLine)); break;
                            case 3: setTriggerType(result.CurrentPanel.CEConfig.Events[eventNum - 1],   parseInt(currentLine)); break;
                            case 4: setTriggerTarget(result.CurrentPanel.CEConfig.Events[eventNum - 1], parseInt(currentLine)); break;
                            case 5: setResetType(result.CurrentPanel.CEConfig.Events[eventNum - 1],     parseInt(currentLine)); break;
                            case 6: setResetTarget(result.CurrentPanel.CEConfig.Events[eventNum - 1],   parseInt(currentLine)); break;
                        }
                    }
                }
            }
        }



        private static void setActionType(CEEvent ceEvent, int value)  => ceEvent.ActionType = (CEActionTypes)(value & 0xff);
        private static void setActionTarget(CEEvent ceEvent, int value) => ceEvent.ActionParam = value;

        private static void setTriggerType(CEEvent ceEvent, int value)
        {
            //ignore msb for trigger type
            ceEvent.TriggerType = (CETriggerTypes)(value & 0x7f);

            //msb is the trigger condition: 0=true, 1=false
            ceEvent.TriggerCondition = (value & 0x80) == 0;
        }

        private static void setTriggerTarget(CEEvent ceEvent, int value)
        {
            //low byte is main target; high byte is secondary target if it's ANDed
            ceEvent.TriggerParam  = value & 0xff;
            ceEvent.TriggerParam2 = value >> 8;
        }

        private static void setResetType(CEEvent ceEvent, int value)
        {
            //ignore msb for reset type
            ceEvent.ResetType = (CETriggerTypes)(value & 0x7f);
            
            //msb is the reset condition: 0=false, 1=true
            ceEvent.ResetCondition = (value & 0x80) != 0;
        }

        private static void setResetTarget(CEEvent ceEvent, int value)
        {
            //low byte is main target; high byte is secondary target if it's ANDed
            ceEvent.ResetParam  = value & 0xff;
            ceEvent.ResetParam2 = value >> 8;
        }

    }
}
