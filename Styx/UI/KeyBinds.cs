using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Styx.KeyConf;
using Styx.Tools;
using UnityEngine;

namespace Styx.UI
{
    public partial class KeyBinds : Form
    {
        public KeyBinds()
        {
            InitializeComponent();
        }

        public static KeyBinds Instance { get; } = new KeyBinds();

        private void Hotkeys_Load(object sender, EventArgs e)
        {
            foreach (var kc in Enum.GetValues(typeof(KeyCode))
                .Cast<KeyCode>()
                .ToList()
                .OrderBy(a => a.ToString()))
                cbKeys.Items.Add(kc);
            lstActive.DisplayMember = "Text";
        }

        private void Hotkeys_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int action;
            if (cbKeys.SelectedIndex > -1 && (action = cbActions.SelectedIndex) > -1)
            {
                var kc = (KeyCode)Enum.Parse(typeof(KeyCode), cbKeys.SelectedItem.ToString(), true);
                if (KeyBind.ActiveBinds.All(a => a.Key != kc))
                {
                    var kb = new KeyBind(kc, action, $"{kc}: {cbActions.Items[action]}");
                    kb.Register();
                    lstActive.Items.Add(kb);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (var ofd = new SaveFileDialog())
            {
                ofd.Title = "Save Styx Hotkeys";
                ofd.Filter = "Styx HotKeys (*.skeys)|*.skeys";
                ofd.DefaultExt = ".sbot";
                ofd.CheckFileExists = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                    SaveKeys(ofd.FileName);
            }
        }

        private void SaveKeys(string file)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            var content = JsonConvert.SerializeObject(GenerateConfig(), Formatting.Indented, settings);

            try
            {
                File.WriteAllText(file, content);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"File write operation failed: {ex.Message}", "Styx",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int index;
            if ((index = lstActive.SelectedIndex) > -1)
            {
                var kb = (KeyBind)lstActive.Items[index];
                kb.Unregister();
                lstActive.Items.Remove(kb);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Load Styx Hotkeys";
                ofd.Filter = "Styx Hotkeys (*.skeys)|*.skeys";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var content = File.ReadAllText(ofd.FileName);
                    LoadKeys(content);
                }
            }
        }

        private void LoadKeys(string content)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            var bc = JsonConvert.DeserializeObject<KeyConfig>(content, settings);

            if (bc == null)
            {
                System.Windows.Forms.MessageBox.Show("Error: Unable to deserialize json (null)",
                    "Styx", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ApplyConfig(bc);
        }

        private void ApplyConfig(KeyConfig bc)
        {
            if (bc == null)
            {
            }
            else
            {
                lstActive.Items.Clear();
                lstActive.Items.AddRange(bc.KeyConfigs.ToArray());
                foreach (var key in lstActive.Items.Cast<KeyBind>().ToList())
                {
                    var kb = new KeyBind(key.Key, key.ActionIndex, key.Text);
                    kb.Register();
                }
            }
        }

        private KeyConfig GenerateConfig()
        {
            var bc = new KeyConfig
            {
                KeyConfigs = lstActive.Items.Cast<KeyBind>().ToList()
            };
            return bc;
        }
    }
}