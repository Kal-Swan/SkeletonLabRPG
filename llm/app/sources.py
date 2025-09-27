from .constants import bg3, daggerheart

bg3_urls = [
    "https://bg3.wiki/wiki/Feats",
    "https://bg3.wiki/wiki/Weapons",
    "https://bg3.wiki/wiki/Shields",
    "https://bg3.wiki/wiki/Armour",
]

intelligence_urls = [
    "https://bg3.wiki/wiki/Wizard",
    "https://bg3.wiki/wiki/Abjuration_School",
    "https://bg3.wiki/wiki/Bladesinging",
    "https://bg3.wiki/wiki/Evocation_School",
    "https://bg3.wiki/wiki/Divination_School",
    "https://bg3.wiki/wiki/Transmutation_School"
]

standard_sources = [
    "https://bg3.wiki/wiki/Dice_rolls",
    "https://bg3.wiki/wiki/Abilities",
    "https://bg3.wiki/wiki/Abilities#Ability_score_modifiers",
    "https://bg3.wiki/wiki/Charisma",
    "https://bg3.wiki/wiki/Wisdom",
    "https://bg3.wiki/wiki/Intelligence",
    "https://bg3.wiki/wiki/Constitution",
    "https://bg3.wiki/wiki/Dexterity",
    "https://bg3.wiki/wiki/Strength",
    "https://bg3.wiki/wiki/Turn-based_mode",
    "https://bg3.wiki/wiki/Feats",
    "https://bg3.wiki/wiki/Armour_Class",
    "https://bg3.wiki/wiki/Weapons",
    "https://bg3.wiki/wiki/Shields",
    "https://bg3.wiki/wiki/Armour",
    "https://bg3.wiki/wiki/Proficiency",
    "https://bg3.wiki/wiki/Spells",
    "https://bg3.wiki/wiki/Heavy_Crossbows",
    "https://bg3.wiki/wiki/Hand_Crossbows",
    "https://bg3.wiki/wiki/Longbows"
]

charisma_classes = [
    "https://bg3.wiki/wiki/Warlock",
    "https://bg3.wiki/wiki/The_Archfey",
    "https://bg3.wiki/wiki/The_Fiend",
    "https://bg3.wiki/wiki/The_Great_Old_One",
    "https://bg3.wiki/wiki/The_Hexblade",
    "https://bg3.wiki/wiki/Sorcerer",
    "https://bg3.wiki/wiki/Draconic_Bloodline",
    "https://bg3.wiki/wiki/Shadow_Magic",
    "https://bg3.wiki/wiki/Storm_Sorcery",
    "https://bg3.wiki/wiki/Paladin",
    "https://bg3.wiki/wiki/Oath_of_Devotion",
    "https://bg3.wiki/wiki/Oath_of_the_Ancients",
    "https://bg3.wiki/wiki/Oath_of_the_Crown",
    "https://bg3.wiki/wiki/Oath_of_Vengeance",
    "https://bg3.wiki/wiki/Oathbreaker",
    "https://bg3.wiki/wiki/Bard",
    "https://bg3.wiki/wiki/College_of_Swords",
    "https://bg3.wiki/wiki/College_of_Glamour",
    "https://bg3.wiki/wiki/College_of_Lore",
    "https://bg3.wiki/wiki/College_of_Valour",
]

wisdom_classes = [
    "https://bg3.wiki/wiki/Cleric",
    "https://bg3.wiki/wiki/Death_Domain",
    "https://bg3.wiki/wiki/Knowledge_Domain",
    "https://bg3.wiki/wiki/Life_Domain",
    "https://bg3.wiki/wiki/Light_Domain",
    "https://bg3.wiki/wiki/Nature_Domain",
    "https://bg3.wiki/wiki/Tempest_Domain",
    "https://bg3.wiki/wiki/Trickery_Domain",
    "https://bg3.wiki/wiki/War_Domain",
    "https://bg3.wiki/wiki/Druid",
    "https://bg3.wiki/wiki/Circle_of_the_Land",
    "https://bg3.wiki/wiki/Circle_of_the_Moon",
    "https://bg3.wiki/wiki/Circle_of_the_Spores",
    "https://bg3.wiki/wiki/Circle_of_the_Stars",
    "https://bg3.wiki/wiki/Monk",
    "https://bg3.wiki/wiki/Way_of_the_Drunken_Master",
    "https://bg3.wiki/wiki/Way_of_the_Four_Elements",
    "https://bg3.wiki/wiki/Way_of_the_Open_Hand",
    "https://bg3.wiki/wiki/Way_of_Shadow"
]

dex_classes = [
    "https://bg3.wiki/wiki/Rogue",
    "https://bg3.wiki/wiki/Thief",
    "https://bg3.wiki/wiki/Swashbuckler",
    "https://bg3.wiki/wiki/Assassin",
    "https://bg3.wiki/wiki/Arcane_Trickster",
    "https://bg3.wiki/wiki/Ranger",
    "https://bg3.wiki/wiki/Gloom_Stalker",
    "https://bg3.wiki/wiki/Hunter",
    "https://bg3.wiki/wiki/Swarmkeeper"

]

strength_classes = [
    "https://bg3.wiki/wiki/Fighter",
    "https://bg3.wiki/wiki/Battle_Master",
    "https://bg3.wiki/wiki/Champion",
    "https://bg3.wiki/wiki/Arcane_Archer",
    "https://bg3.wiki/wiki/Eldritch_Knight",
    "https://bg3.wiki/wiki/Barbarian",
    "https://bg3.wiki/wiki/Berserker",
    "https://bg3.wiki/wiki/Giant_(barbarian_subclass)",
    "https://bg3.wiki/wiki/Wildheart",
    "https://bg3.wiki/wiki/Rage"
]

all_sources = {
    bg3: [*standard_sources, *wisdom_classes, *dex_classes, *strength_classes, *charisma_classes, *intelligence_urls],
    daggerheart: ["C:\\PDF\\streamlined_daggerheart_core_rulebook_2.pdf", "C:\\PDF\\Daggerheart_Assassin_Class.pdf"]
}