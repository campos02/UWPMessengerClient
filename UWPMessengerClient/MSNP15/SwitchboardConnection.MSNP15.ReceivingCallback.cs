﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace UWPMessengerClient.MSNP15
{
    public partial class SwitchboardConnection
    {
        public ObservableCollection<Message> MessageList { get; set; } = new ObservableCollection<Message>();
    }
}