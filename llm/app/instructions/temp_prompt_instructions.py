instruction = """
When the user asks a question about a character build, based the RPG system return a structured JSON object describing the build.

Your JSON should include properties that are relevant to that RPG system and from what you learnt.

Only return a single raw JSON object, no markdown or explanation. Use camelCase for keys. I have included a skeleton JSON object to start with, it only includes a "buildName" field for each build. Include a "reason" field for each major choice. Also, include other fields that are relevant.

Adapt the structure based on the system.

Be consistent and realistic in structure and content, and fill in all fields with sensible values.

Use the provided game data to answer the following player question clearly and helpfully.

I have provided relevant game content.

Your task is to generate the most optimized build possible given the level cap for this RPG system. The build should be returned in structured JSON format as described in the format instructions below. Please provide multiple build options with this format, an array so that the user has multiple choices to select from.

Follow these principles strictly:

### Build Strategy Guidelines

1. **Content Source Awareness**
   - Some pages may contain both mechanical content and lore or tactics about enemies. You must differentiate between "buildable content" and "reference content."
   - Do not assume all text relates to character creation. Cross-check the section heading or title before using any ability, subclass, or trait.    

2. **Learning Material**    
   - Use Only Player-Available Options
   - The content provided may include both player character options and non-player content (such as enemies, monsters, and adversaries).
   - Only use character classes, subclasses, and abilities that are available to player characters.
   - Do not use abilities or subclasses found only in stat blocks or adversary descriptions, even if they appear related.
   - Always check the heading or section title of the content. If the content is part of an "Enemies", "Adversaries", "Combat Strategies", or "NPC" section, ignore class names or features from that section.
   - If unsure whether something is available to the player, prefer to exclude it. 

3. **Question**
   - Do not take every word in the question literally, focus on the intent. 
   - Do not try to match it to words in the data provided to learn from but rather the intent of the question.                                                                                                        

4. **Unify Stats, Classes, and Weapons**
   - Do not recommend weapons that scale with attributes not prioritized in the build.
   - Only recommend weapons that scale with the highest attribute(s) used for attack rolls and damage.
   - Compare weapon scaling with stat scaling, compare them with build's highest stats. Exclude or downrank mismatches.
   - Only include weapons and combat styles that are usable by the classes selected, it is not mandatory to include weapons if you feel it does not benefit the build.
   - Only choose abilities that directly enhance those weapons or combat mechanics.
   - Avoid conflicting stat priorities.

5. **Synergize Classes and Subclasses**
   - Only include classes that **directly contribute** to the goal stated in the question (e.g., burst damage, sustained defense, utility).
   - Ensure subclasses are accessed at the **correct levels** (e.g., most at level 3).
   - Consider subclass features that enable combat burst, action economy, status effects, survivability, etc., depending on the question.

6. **Respect Action Economy**
   - Consider how many actions are available per turn.
   - Consider pre-combat features vs in-combat performance.

7. **Reasoning**
   - Justify all major choices.
   - Explain how they work together to fulfill the player’s stated goal.

8. **Combat Style Consistency**
   - Do not combine mechanics that conflict in combat focus. For example:

9. **Multiclassing**
    - If multiclassing, ensure it is done to enhance the build's core combat effectiveness.
    - Avoid multiclassing that dilutes the build's focus or creates conflicting stat requirements.
    - Justify each multiclass choice in terms of how it enhances the build's combat capabilities.

Format Instructions:
{format_instructions}   
    
Game Data:
{context}
                                                                                                   
Player Question: 
{question}
                                                   
Answer:"""


bg3_instruction = """
Use the provided game data to answer the following player question clearly and helpfully.

I have provided wiki pages of relevant game content (classes, subclasses, feats, ability scores, weapons, etc.).

Your task is to generate the most optimized build possible given a level cap of 12. The build should be returned in structured JSON format as described in the format instructions below. Please provide multiple build options with this format, an array so that the user has multiple choices to select from.

Follow these principles strictly:

### Build Strategy Guidelines

1. **Unify Stats, Classes, and Weapons**
   - Only include weapons and combat styles that are usable by the classes selected, it is not mandatory to include weapons if you feel it does not benefit the build.
   - Only choose feats that directly enhance those weapons or combat mechanics.
   - Avoid conflicting stat priorities (e.g., don't use both Strength and Dexterity builds in the same character unless intentional and justified).
   - If a feat or fighting style depends on a specific stat (e.g., GWM needs Strength, Sharpshooter needs Dexterity), the build must allocate that stat accordingly.

2. **Synergize Classes and Subclasses**
   - Only include classes that **directly contribute** to the goal stated in the question (e.g., burst damage, sustained defense, utility).
   - Ensure subclasses are accessed at the **correct levels** (e.g., most at level 3).
   - Consider subclass features that enable combat burst, action economy, status effects, survivability, etc., depending on the question.

3. **Respect Action Economy**
   - Avoid redundant use of Bonus Actions unless they’re part of the strategy (e.g., Monk Flurry + Rogue Cunning Action may conflict).
   - Consider how many actions, bonus actions, and reactions are available per turn.
   - Consider pre-combat features vs in-combat performance (e.g., Assassin's auto-crits only apply in surprise).

4. **Feats**
   - Only include feats that can actually be selected based on the total character level.
   - Feats must directly enhance the core combat style of the build.
   - Do not include multiple conflicting combat-style feats (e.g., Sharpshooter and GWM on a Monk).
   - If you select a feat like Tavern Brawler, explain how the character will consistently trigger its bonus damage (e.g., via thrown objects, unarmed attacks, etc.).
   - If you feel most feats do not synchonize does not benefit the answer, you can always fall back to ability improvement feats (ASIs) to boost primary stats as these help with attack rolls and some with damage rolls (e.g. strength, dexterity etc).

5. **Ability Scores**
   - Allocate stats to maximize the primary combat effectiveness of the chosen build.
   - Avoid over-distribution. Focus on stats that enhance key class features and feats.
   - Ability Score Improvements (ASIs) should be used efficiently — don't waste stat points on abilities that aren't used.

6. **Reasoning**
   - Justify all major choices: classes, subclasses, feats, weapons, and stats.
   - Explain how they work together to fulfill the player’s stated goal (e.g., “burst damage” or “defensive control”).

7. **Combat Style Consistency**
   - Build around a unified combat approach: either Strength-based (e.g., Great Weapon Master, heavy weapons), Dexterity-based (e.g., finesse weapons, archery), or unarmed/throwing-based (e.g., Tavern Brawler).
   - Do not combine feats or class mechanics that conflict in combat focus. For example:
     - Do not use Champion crit mechanics with unarmed Monk attacks.
     - Do not take Great Weapon Master unless you use heavy weapons and have the Strength to back it up.
     - Avoid feat redundancy or suboptimal usage (e.g., Martial Adept on a class that rarely uses maneuvers).
   - Explain clearly how the build sustains consistent damage output within its chosen style across multiple turns.
                                                   
 8. **Multiclassing**
    - If multiclassing, ensure it is done to enhance the build's core combat effectiveness.
    - Avoid multiclassing that dilutes the build's focus or creates conflicting stat requirements.
    - Justify each multiclass choice in terms of how it enhances the build's combat capabilities.

9 **Extra Attack**
   - Fighter classes like Fighter, Barbarian and Monk (and some others) already have Extra attack by multiclassing these, they don't stack
                                                                                                   
                                                   

### Format Instructions:
{format_instructions}

Game Data:
{context}
                                                                                                   
Player Question: 
{question}
                                                   
Answer:"""

daggerheart_instruction = """
Use the provided game data to answer the following player question clearly and helpfully.

I have provided pdf of relevant game content (classes, subclasses, domains, character traits, weapons, etc.).

Your task is to generate the most optimized build possible given a level cap of 10 The build should be returned in structured JSON format as described in the format instructions below. Please provide multiple build options with this format, an array so that the user has multiple choices to select from.

Follow these principles strictly:

### Build Strategy Guidelines

1. **Unify Stats, Classes, and Weapons**
   - Only include weapons and combat styles that are usable by the classes selected, it is not mandatory to include weapons if you feel it does not benefit the build.
   - Only choose domain that directly enhance those weapons, combat mechanics, classes or subclasses.
   - Avoid conflicting stat priorities
   - If a domain or fighting style depends on a specific stat, the build must allocate that stat accordingly.

2. **Synergize Classes and Subclasses**
   - Only include classes that **directly contribute** to the goal stated in the question (e.g., burst damage, sustained defense, utility).
   - Ensure subclasses are accessed at the **correct levels**.
   - Consider subclass features that enable combat burst, action economy, status effects, survivability, etc., depending on the question.

3. **Respect Action Economy**
   - Consider how many actions are available per turn.
   - Consider pre-combat features vs in-combat performance.

4. **Domains**
   - Only include domains that can actually be selected based on the total character level.
   - Domains must directly enhance the core combat style of the build.
   - Do not include multiple conflicting combat-style domains (e.g., Sharpshooter and GWM on a Monk).

5. **Character traits**
   - Allocate stats to maximize the primary combat effectiveness of the chosen build.
   - Avoid over-distribution. Focus on stats that enhance key class features and domains.
   - Character trait Improvements should be used efficiently — don't waste stat points on abilities that aren't used.

6. **Reasoning**
   - Justify all major choices: classes, subclasses, domains, weapons, and stats.
   - Explain how they work together to fulfill the player’s stated goal (e.g., “burst damage” or “defensive control”).

7. **Combat Style Consistency**
   - Build around a unified combat approach: either Strength-based, Finesse-based etc.
   - Do not combine domain or class mechanics that conflict in combat focus.
   - Explain clearly how the build sustains consistent damage output within its chosen style across multiple turns.
                                                   
 8. **Multiclassing**
    - If multiclassing, ensure it is done to enhance the build's core combat effectiveness.
    - Avoid multiclassing that dilutes the build's focus or creates conflicting stat requirements.
    - Justify each multiclass choice in terms of how it enhances the build's combat capabilities.                                                                                  
                                                   

### Format Instructions:
{format_instructions}

Game Data:
{context}
                                                                                                   
Player Question: 
{question}
                                                   
Answer:
"""

instructions = """
When the user asks a question about a character build, based the RPG system return a structured JSON object describing the build.

Your JSON should include properties that are relevant to that RPG system and from what you learnt.

Only return a single raw JSON object, no markdown or explanation. Use camelCase for keys. Include a "reason" field for each major choice.

Adapt the structure based on the system.

Be consistent and realistic in structure and content, and fill in all fields with sensible values.
"""

