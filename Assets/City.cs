using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{

    public int population = 3;
    public float FoodBankGrowthNeeded { get { return population * 10 + 5; } }
    public float FoodBank = 0;
    public List<TileCell> workedTiles = new List<TileCell>();
    List<Buildings> buildings = new List<Buildings>();
    Dictionary<Zones, List<TileCell>> ZoneCells = new Dictionary<Zones, List<TileCell>>();
    List<ProductionOptions> options;
    float currentProductionBank = 0;
    ProductionOptions currentProductionOption;
    int BASE_FOOD = 3;
    int BASE_PRODUCTION = 3;
    public TileCell cell;
    public GameObject CityUI;
    public bool active = false;
    public List<TileCell> peoplePlaceableCells = new List<TileCell>();
    public bool placingPop = false;
    TileGrid grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<TileGrid>();
        FoodBank = FoodBankGrowthNeeded;
        options = GenProductionOptions();
        currentProductionOption = options.First(p => p.name == "None");
        CityUI = GameObject.Find("Canvas").transform.Find("City").gameObject;
        List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();
        foreach (var option in options)
            optionData.Add(new TMP_Dropdown.OptionData() { text = option.name });
        GameObject.Find("Canvas").transform.Find("City").transform.Find("Dropdown").GetComponent<TMP_Dropdown>().options = optionData;
    }
    public void ActivateCity()
    {
        active = true;
        CityUI.SetActive(true);
    }
    public void PlaceZone(TileCell cell, Zones zone)
    {
        cell.zone = zone;
        if (!ZoneCells.ContainsKey(zone))
            ZoneCells.Add(zone, new List<TileCell>());
        ZoneCells[zone].Add(cell);
    }
    // Update is called once per frame
    void Update()
    {
        if (!active)
            return;
        currentProductionOption = options[GameObject.Find("Canvas").transform.Find("City").transform.Find("Dropdown").GetComponent<TMP_Dropdown>().value];
        UpdateCityStats();
    }
    public void UpdateCityStats()
    {
        CityUI.transform.Find("CityStats").transform.Find("ProductionSlider").gameObject.GetComponent<Slider>().value = currentProductionOption != null ? (currentProductionBank / (float)currentProductionOption.production_cost) : 0;
        CityUI.transform.Find("CityStats").transform.Find("FoodSlider").GetComponent<Slider>().value = FoodBank / FoodBankGrowthNeeded;
        CityUI.transform.Find("CityStats").transform.Find("Population").GetComponent<TextMeshProUGUI>().text = population.ToString();
    }
    public void PopulationPlacement()
    {
        if (!placingPop)
        {
            placingPop = true;
            //this should close when you end the turn
            peoplePlaceableCells = new List<TileCell>();
            //get all tiles that are within 3
            foreach (var zone in ZoneCells)
                foreach (TileCell cell in zone.Value)
                        peoplePlaceableCells.AddRange(grid.CellsInRadius(cell, 3));
            
            //hide all children
            foreach (Transform child in GameObject.Find("WorldSpaceCanvas").transform.Find("TileUI").transform)
            {
                child.gameObject.SetActive(false);
            }
            foreach (var workableCells in peoplePlaceableCells)
            {
                if (workableCells.feature != TerrainFeature.Mountains && workableCells != cell)
                {
                    workableCells.cellUI.gameObject.SetActive(true);
                    workableCells.cellUI.GetComponent<TextMeshProUGUI>().text = "F:" + cell.tileFoodProduction + " P:" + cell.tileProduction;
                }
            }
        }
        else
        {
            placingPop = false;
            //hide all children
            foreach (Transform child in GameObject.Find("WorldSpaceCanvas").transform.Find("TileUI").transform)
            {
                child.gameObject.SetActive(false);
            }
        }

    }
    public void ASDF(GameObject obj)
    {

    }
    public void EndTurn()
    {
        if (!active)
            return;
        EndTurnFoodProduction();
        EndTurnProduction();
    }
    public void EndTurnProduction()
    {
        currentProductionBank += PerTurnProduction();
        if (currentProductionBank >= currentProductionOption.production_cost)
        {
            FinishProduction();
        }
    }

    public float PerTurnProduction()
    {
        float Production = BASE_PRODUCTION;
        foreach (TileCell cell in workedTiles)
        {
            Production += cell.tileProduction;
        }
        if (buildings.Contains(Buildings.Library))
            Production *= 1.1f;
        return Production;
    }
    public void EndTurnFoodProduction()
    {
        FoodBank += PerTurnFoodProduction();
        if (FoodBank >= FoodBankGrowthNeeded)
        {
            FoodBank = FoodBank - FoodBankGrowthNeeded;
            population++;
        }
    }
    public float PerTurnFoodProduction()
    {
        float foodProduction = BASE_FOOD;
        foreach (TileCell cell in workedTiles)
        {
            foodProduction += cell.tileFoodProduction;
        }
        if (buildings.Contains(Buildings.Library))
            foodProduction *= 1.1f;
        foodProduction -= population * 2;
        return foodProduction;
    }
    public void FinishProduction()
    {
        if (currentProductionOption.name == "Houses")
        {

        }
        else if (currentProductionOption.name == "Apartment")
        {

        }
        else if (currentProductionOption.name == "Mansion")
        {

        }
        else if (currentProductionOption.name == "Granary")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Granary);
        }
        else if (currentProductionOption.name == "Library")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Library);
        }
        else if (currentProductionOption.name == "Lighthouse")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Lighthouse);
        }
        else if (currentProductionOption.name == "Shipyard")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Shipyard);
        }
        else if (currentProductionOption.name == "Monument")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Monument);
        }
        else if (currentProductionOption.name == "Shrine")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Shrine);
        }
        else if (currentProductionOption.name == "Temple")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Temple);
        }
        else if (currentProductionOption.name == "Arena")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Arena);

        }
        else if (currentProductionOption.name == "Ampitheater")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Ampitheater);
        }
        else if (currentProductionOption.name == "Market")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Market);
        }
        else if (currentProductionOption.name == "Workshop")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Workshop);
        }
        else if (currentProductionOption.name == "Armory")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Armory);
        }
        else if (currentProductionOption.name == "Potter")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.Potter);
        }
        else if (currentProductionOption.name == "StoneMason")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.StoneMason);
        }
        else if (currentProductionOption.name == "LumberMill")
        {
            options.Remove(options.First(p => p.name == currentProductionOption.name));
            RefreshDDLBuildOptions();
            buildings.Add(Buildings.LumberMill);
        }
        else if (currentProductionOption.name == "Worker")
        {

        }
        else if (currentProductionOption.name == "Explorer")
        {

        }
        currentProductionBank -= currentProductionOption.production_cost;
        currentProductionOption = options.First(p => p.name == "None");
        GameObject.Find("Canvas").transform.Find("City").transform.Find("Dropdown").GetComponent<TMP_Dropdown>().SetValueWithoutNotify(0);

    }
    public List<ProductionOptions> GenProductionOptions()
    {
        List<ProductionOptions> options = new List<ProductionOptions>();
        options.Add(new ProductionOptions() { name = "None", production_cost = 999999 });
        options.Add(new ProductionOptions() { name = "Houses", production_cost = 80 });
        options.Add(new ProductionOptions() { name = "Apartment", production_cost = 250 });
        options.Add(new ProductionOptions() { name = "Mansion", production_cost = 300 });
        options.Add(new ProductionOptions() { name = "Granary", production_cost = 65 });
        options.Add(new ProductionOptions() { name = "Library", production_cost = 100 });
        options.Add(new ProductionOptions() { name = "Lighthouse", production_cost = 80 });
        options.Add(new ProductionOptions() { name = "Shipyard", production_cost = 200 });
        options.Add(new ProductionOptions() { name = "Monument", production_cost = 60 });
        options.Add(new ProductionOptions() { name = "Shrine", production_cost = 80 });
        options.Add(new ProductionOptions() { name = "Temple", production_cost = 150 });
        options.Add(new ProductionOptions() { name = "Arena", production_cost = 300 });
        options.Add(new ProductionOptions() { name = "Ampitheater", production_cost = 300 });
        options.Add(new ProductionOptions() { name = "Market", production_cost = 200 });
        options.Add(new ProductionOptions() { name = "Workshop", production_cost = 150 });
        options.Add(new ProductionOptions() { name = "Armory", production_cost = 200 });
        options.Add(new ProductionOptions() { name = "Potter", production_cost = 200 });
        options.Add(new ProductionOptions() { name = "StoneMason", production_cost = 200 });
        options.Add(new ProductionOptions() { name = "LumberMill", production_cost = 200 });
        options.Add(new ProductionOptions() { name = "Worker", production_cost = 50 });
        options.Add(new ProductionOptions() { name = "Explorer", production_cost = 150 });
        return options;
    }
    public void RefreshDDLBuildOptions()
    {
        List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();
        foreach (var option in options)
            optionData.Add(new TMP_Dropdown.OptionData() { text = option.name });
        GameObject.Find("Canvas").transform.Find("City").transform.Find("Dropdown").GetComponent<TMP_Dropdown>().options = optionData;
    }
}
[Serializable]
public enum TileImprovments
{
    None,
    Watermill,
    Farm,
    Pasture,
    Mine,
    WoodCutter,
    Fisher
}
[Serializable]
public enum Zones
{
    None,
    Civic,
    Commerce,
    Industrial,
    Cultural,
    Religious,
    Warehouse,
    Harbor
}
[Serializable]
public enum Buildings
{
    Library,
    Granary,
    Lighthouse,
    Shipyard,
    Monument,
    Shrine,
    Temple,
    Arena,
    Ampitheater,
    Market,
    Workshop,
    Armory,
    Potter,
    StoneMason,
    LumberMill
}
public class ProductionOptions
{
    public string name;
    public int production_cost;

}
