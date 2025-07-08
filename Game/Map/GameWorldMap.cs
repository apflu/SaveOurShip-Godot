using Godot;
using System;
using System.Collections.Generic;

public partial class GameWorldMap : Node2D
{
    [Export]
    public PackedScene Player_Character_Scene { get; set; }
    [Export]
    public PackedScene Monster_Character_scene { get; set; }
    [Export]
    public int Debug_Monster_Spawn_Count = 5; // 用于调试的怪物生成数量

    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    private List<string> _availableLocales = []; // 用于存储可用语言列表

    public override void _Ready()
    {
        _rng.Randomize(); // 初始化随机数生成器，使用当前时间作为种子
        SpawnPlayer(); // 生成玩家角色

        InitLanguage(); // 初始化语言选项

        // Debug: 创建一个带Tr()文本的标签
        Label label = new Label();
        label.Text = Tr("Hi, this is a test label!");
        label.Position = new Vector2(100, 100); // 设置标签位置
        AddChild(label); // 将标签添加到当前场景树中
    }

    private void SpawnPlayer()
    {
        // 确保Player_Character_Scene已经设置
        if (Player_Character_Scene == null)
        {
            GD.PrintErr("PlayerCharacterScene is not assigned in the Inspector!");
            return;
        }

        // 1. 实例化PlayerCharacter场景
        Node2D playerInstance = Player_Character_Scene.Instantiate<Node2D>();

        // 2. 设置节点名称
        playerInstance.Name = "Player";

        // 3. 将其添加为当前场景的子节点
        AddChild(playerInstance);
        playerInstance.Position = new Vector2(100, 100); // 设置初始位置

        GD.Print($"Player node named '{playerInstance.Name}' added to the scene.");
    }

    private void SpawnMonsters()
    {
        var _navigationRegion = GetNode<NavigationRegion2D>("NavigationRegion"); // 确保路径正确
        // 1. 检查 NavigationRegion2D 及其 NavigationPolygon 是否有效
        if (_navigationRegion.NavigationPolygon == null)
        {
            GD.PrintErr("Error: NavigationRegion2D does not have a NavigationPolygon resource assigned.");
            return;
        }

        // 检查 NavigationPolygon 是否包含有效的多边形（意味着它被烘焙了）
        if (_navigationRegion.NavigationPolygon.GetPolygonCount() == 0)
        {
            GD.PrintErr("Error: NavigationPolygon is empty or not baked. Monster spawning will fail.");
            return;
        }

        // 2. 获取 NavigationRegion2D 的 RID
        // 这是一个关键步骤，确保我们操作的是正确的导航区域
        Rid regionRid = _navigationRegion.GetRid(); // 获取 NavigationRegion2D 的 RID
        GD.Print($"NavigationRegion2D RID: {regionRid}");
        if (regionRid == default) // 检查 RID 是否有效
        {
            GD.PrintErr("Error: NavigationRegion2D RID is invalid. Is it added to the scene tree?");
            return;
        }

        // 3. 开始生成怪物
        for (int i = 0; i < Debug_Monster_Spawn_Count; i++)
        {
            Node2D monster = Monster_Character_scene.Instantiate<Node2D>();

            if (monster == null)
            {
                GD.PrintErr("Failed to instantiate monster scene.");
                continue;
            }

            monster.AddToGroup("Monster");

            // 获取 NavigationRegion2D 内部的随机点
            Vector2 randomSpawnPoint = NavigationServer2D.RegionGetRandomPoint(regionRid, _navigationRegion.NavigationLayers, true);

            GD.Print($"Random spawn point for monster: {randomSpawnPoint}");

            // 检查获取到的点是否是有效点（非零）
            // 如果导航网格非常小或无效，可能会返回 Vector2.Zero
            if (randomSpawnPoint == Vector2.Zero && _navigationRegion.NavigationPolygon.GetPolygonCount() > 0)
            {
                GD.PrintErr($"Warning: Failed to get a random spawn point within NavigationRegion2D. Try increasing its size or checking the baking process. Attempt {i + 1}");
                // 可以选择在这里跳过这次生成，或者尝试重新获取
                continue;
            }

            // 设置怪物的位置 (使用 GlobalPosition 确保在正确的世界坐标)
            monster.GlobalPosition = randomSpawnPoint;

            AddChild(monster); // 将怪物添加到当前场景树
            GD.Print($"Spawned monster '{monster.Name}' at position: {monster.Position}");
        }
    }

    public void InitLanguage()
    {
        // 获取在UILayer下要填充的OptionButton节点
        var _languageOptionButton = GetNode<OptionButton>("UILayer/OptionButton");

        // 创建一个存储所有可用语言的列表，并用TranslationServer.GetLoadedLocales()作为string[]来填充它
        _availableLocales = [.. TranslationServer.GetLoadedLocales()];


        // 确保列表非空，并且包含至少一种语言
        if (_availableLocales.Count == 0)
        {
            GD.PrintErr("No translation locales found in Project Settings. Please add .po files.");
            // 添加一个默认语言
            _availableLocales.Add("en"); // fallback
            // 禁用 OptionButton
            _languageOptionButton.Disabled = true;
        }

        // 2. 清空并填充 OptionButton
        _languageOptionButton.Clear(); // 清除所有现有项
        foreach (string localeCode in _availableLocales)
        {
            // 对于用户友好的显示，可以考虑将语言代码映射到完整的语言名称
            // 例如： "en" -> "English", "zh_CN" -> "简体中文"
            _languageOptionButton.AddItem(TranslationServer.GetLocaleName(localeCode));
        }

        // 3. 设置 OptionButton 的当前选中项为当前活跃语言
        string currentLocale = TranslationServer.GetLocale();
        int currentIndex = _availableLocales.IndexOf(currentLocale);
        if (currentIndex != -1)
        {
            _languageOptionButton.Selected = currentIndex;
        }
        else
        {
            // 如果当前语言不在列表中（例如，游戏启动时是系统语言，但没有对应的翻译文件）
            // 尝试设置第一个语言为默认选中，或者根据项目逻辑处理
            GD.PrintErr($"Current locale '{currentLocale}' not found in loaded translations. Setting first option.");
            _languageOptionButton.Selected = 0; // 默认选中第一个
            // 考虑在此处调用SetNewLanguage(_availableLocales[0]); 以确保同步
        }
    }

    public void OnButtonPressed(long index)
    {
        if (index >= 0 && index < _availableLocales.Count)
        {
            string selectedLocaleCode = _availableLocales[(int)index];
            UpdateLanguage(selectedLocaleCode);
        }
    }

    private void UpdateLanguage(string locale)
    {
        if (TranslationServer.GetLocale() != locale)
        {
            TranslationServer.SetLocale(locale); // 设置新的语言环境
            GD.Print($"Language changed to: {locale}");
        }
        else
        {
            GD.Print("Language is already set to the requested locale.");
        }
    }
}
