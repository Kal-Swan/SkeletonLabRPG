instruction = """
Your answer includes relevant information to that RPG system and from what you learnt.

I have included a json object, it only includes a "name" field for each build. It includes a "reason" field for each build too.
When the user asks a question about a character build, based the RPG system, return a structured answer in the "template" field with appropriate headings, sub-headings, line breaks and spacing.
But don't include asterisks, just plain text. Also, give enough information on character attributes, classes, subclasses, abilities, weapons, spells etc. These could be called something else based on the rpg system.

Give detailed answers, in the same format and details for each build with appropriate data based on the RPG system.

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
   - Do not give vague answers e.g. "Powerful feat/skill" etc.

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
