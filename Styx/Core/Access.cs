using System;
using Styx.Botting;
using Styx.Tools;

namespace Styx.Core
{
    public class Access
    {
        public int NewAccess = -1;
        public string NewName;
        public string NewTitle;
        public int RealAccess = -1;

        public void CheckTitles()
        {
            try
            {
                if (AutoLogin.IsLoggedIn && AutoLogin.IsLoaded)
                {
                    if (NewName != null)
                    {
                        if (Entities.GetInstance().me.name != NewName)
                            Entities.GetInstance().me.name = NewName;
                        if (UIGame.Instance.PortraitPlayer.lblName.text != NewName.ToTitleCase())
                            UIGame.Instance.PortraitPlayer.lblName.text = NewName.ToTitleCase();
                    }

                    if (NewTitle != null)
                        if (Entities.GetInstance().me.TitleName != NewTitle)
                            Entities.GetInstance().me.TitleName = NewTitle;

                    if (NewAccess != 0)
                    {
                        if (Entities.GetInstance().me.AccessLevel != NewAccess)
                            Entities.GetInstance().me.AccessLevel = NewAccess;
                        if (NewAccess >= 50)
                        {
                            UIGame.Instance.PortraitPlayer.Flare.gameObject.SetActive(true);
                            UIGame.Instance.PortraitPlayer.FLareLabel.gameObject.SetActive(true);
                            UIGame.Instance.PortraitPlayer.Kickstarter.gameObject.SetActive(false);
                            if (NewAccess >= 100)
                            {
                                if (UIGame.Instance.PortraitPlayer.Flare.color != InterfaceColors.Names.Staff)
                                    UIGame.Instance.PortraitPlayer.Flare.color = InterfaceColors.Names.Staff;
                                if (UIGame.Instance.PortraitPlayer.FLareLabel.text != "S")
                                    UIGame.Instance.PortraitPlayer.FLareLabel.text = "S";
                            }
                            else if (NewAccess >= 60)
                            {
                                if (UIGame.Instance.PortraitPlayer.Flare.color != InterfaceColors.Names.Mod)
                                    UIGame.Instance.PortraitPlayer.Flare.color = InterfaceColors.Names.Mod;
                                if (UIGame.Instance.PortraitPlayer.FLareLabel.text != "M")
                                    UIGame.Instance.PortraitPlayer.FLareLabel.text = "M";
                            }
                            else if (NewAccess >= 55)
                            {
                                if (UIGame.Instance.PortraitPlayer.Flare.color != InterfaceColors.Names.WhiteHat)
                                    UIGame.Instance.PortraitPlayer.Flare.color = InterfaceColors.Names.WhiteHat;
                                if (UIGame.Instance.PortraitPlayer.FLareLabel.text != "W")
                                    UIGame.Instance.PortraitPlayer.FLareLabel.text = "W";
                            }
                            else if (NewAccess >= 50)
                            {
                                if (UIGame.Instance.PortraitPlayer.Flare.color != InterfaceColors.Names.Tester)
                                    UIGame.Instance.PortraitPlayer.Flare.color = InterfaceColors.Names.Tester;
                                if (UIGame.Instance.PortraitPlayer.FLareLabel.text != "T")
                                    UIGame.Instance.PortraitPlayer.FLareLabel.text = "T";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(@"Something went wrong, this is probably due to a game update. Please notify us in our Discord server.", "Styx Error: Flair");
            }
        }

        public void Subscribe()
        {
            if (AutoLogin.IsLoggedIn && AutoLogin.IsLoaded)
            {
                if (RealAccess == -1)
                {
                    RealAccess = World.Me.AccessLevel;
                    NewAccess = World.Me.AccessLevel;
                    CheckPlayerAccess();
                }

                CheckTitles();
            }
        }

        private void CheckPlayerAccess()
        {
            if (RealAccess >= 60)
            {
                MessageBox.Show("Styx", $"Sorry {NewName}, you're not allowed to use Styx.");
                if (AutoLogin.IsLoggedIn) Game.Instance.Logout();
            }
        }
    }
}