using Godot;
using System;

public partial class MenuStore : Control
{
    [Export] public MenuFadeComponent MenuFadeComponent;
    [Export] public HUDDialogSystem HUD;
    [Export] public BuyPowerUpContainer BuyPowerUpContainer;
    [Export] public UpgradePowerUpContainer UpgradePowerUpContainer;
    [Export] public UpgradeShipContainer UpgradeShipContainer;
    [Export] public AssignContainerBox AssignContainerBox;
    [Export] public Button ReturnButton;
    [Export] public Button EquipButton;
    [Export] public Button BuyButton;
    [Export] public Button UpgradeButton;

    private string _selectedWeaponKey = "";

    public override void _Ready()
    {
        WireGrid();
        WireSpecials();
        WireCards();
        WireButtons();
    }

    private void WireGrid()
    {
        foreach (var storeItem in GetTree().GetNodesInGroup("store_item_panels"))
        {
            if (storeItem is StoreItemPanel panel)
            {
                panel.StoreItemClicked += OnStoreItemClicked;
            }
        }
    }

    private void WireSpecials()
    {
        foreach (var specialItem in GetTree().GetNodesInGroup("special_item_buttons"))
        {
            if (specialItem is SpecialUpgradeButton button)
            {
                button.SpecialItemClicked += OnSpecialItemClicked;
            }
        }
    }

    private void WireCards()
    {
        BuyPowerUpContainer.PurchaseMessage += OnPurchaseMessage;
        UpgradePowerUpContainer.UpgradeMessage += OnUpgradeMessage;
        UpgradeShipContainer.UpgradeMessage += OnUpgradeMessage;
    }

    private void WireButtons()
    {
        EquipButton.Pressed += OnEquipPressed;
        BuyButton.Pressed += OnBuyPressed;
        UpgradeButton.Pressed += OnUpgradePressed;
        ReturnButton.Pressed += OnReturnPressed;
    }

    private void OnSpecialItemClicked(string specialItemKey)
    {
        if ("Ship".Equals(specialItemKey))
        {
            UpgradePowerUpContainer.Visible = false;
            BuyPowerUpContainer.Visible = false;
            UpgradeShipContainer.Visible = true;
        }
        else
        {
            GD.Print(specialItemKey);
            _selectedWeaponKey = specialItemKey;
            BuyPowerUpContainer.Visible = false;
            UpgradeShipContainer.Visible = false;
            UpgradePowerUpContainer.LoadContainer(_selectedWeaponKey);
            UpgradePowerUpContainer.Visible = true;
        }
        EquipButton.Visible = false;
        BuyButton.Visible = false;
        UpgradeButton.Visible = false;
    }

    private void OnStoreItemClicked(string storeItemKey)
    {
        _selectedWeaponKey = storeItemKey;
        EquipButton.Visible = true;
        BuyButton.Visible = true;
        UpgradeButton.Visible = true;
        UpgradeShipContainer.Visible = false;
        UpgradePowerUpContainer.Visible = false;
        BuyPowerUpContainer.LoadContainer(_selectedWeaponKey);
        BuyPowerUpContainer.Visible = true;
    }

    private void OnPurchaseMessage(string message)
    {
        _ = HUD.PopUpMessage(Char.FRIEND, Mood.FRIEND.Default, message);
    }

    private void OnUpgradeMessage(string message)
    {
        _ = HUD.PopUpMessage(Char.FRIEND, Mood.FRIEND.Default, message);
    }

    private void OnEquipPressed()
    {
        AssignContainerBox.OpenFor(_selectedWeaponKey);
    }

    private void OnBuyPressed()
    {
        UpgradeShipContainer.Visible = false;
        UpgradePowerUpContainer.Visible = false;
        BuyPowerUpContainer.LoadContainer(_selectedWeaponKey);
        BuyPowerUpContainer.Visible = true;
    }

    private void OnUpgradePressed()
    {
        BuyPowerUpContainer.Visible = false;
        UpgradeShipContainer.Visible = false;
        UpgradePowerUpContainer.LoadContainer(_selectedWeaponKey);
        UpgradePowerUpContainer.Visible = true;
    }

    private async void OnReturnPressed()
    {
        await MenuFadeComponent.FadeOutAsync();
        await G.GF.FadeToSceneBasic(G.GF.MenuLevelsScene);
    }
}
