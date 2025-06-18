<script lang="ts">
	import {
		CharacterAttributeDamageSchema,
		type CharacterAttribute,
		type CharacterAttributeDamage,
		type CharacterDamage
	} from '@models/character/character-attribute-schema';
	import Form from './form.svelte';
	import InputField from './input-field.svelte';
	import TextButton from './text-button.svelte';
	import { calculateDamage } from '@helpers/damage-calulations';
	import { WeaponType } from '@models/weapon-type';
	import Label from './label.svelte';
	import { DamageType } from '@models/damage-type';
	import SelectField from './select-field.svelte';
	import { ClassType } from '@models/class-type';
	import { Actions } from '@models/actions';
	import { clientFetch, type Result } from '@helpers/client-fetch';
	import type { ErrorResponse } from '@models/error-response';
	import Notification from './notification.svelte';
	// import { type CharacterAttributeRequest } from '../../routes/request';

	let { id, classType } = $props<{ id: string; classType: ClassType }>();
	let request = $state<CharacterAttributeDamage[]>([]);

	let fieldErrors = $state<Record<string, string[]>>({});

	const characterAttributeData = $state<CharacterAttribute>({
		level: 0,
		numDice: 0,
		dieSize: 0,
		strengthModifier: 8,
		dexterityModifier: 8,
		constitutionModifier: 8,
		intelligenceModifier: 8,
		wisdomModifier: 8,
		charismaModifier: 8,
		weaponBonus: 0,
		criticalHit: false,
		advantage: false,
		extraAttack: 0,
		damageType: DamageType.Slashing,
		weaponType: WeaponType.Strength,
		classType: classType,
		characterClassId: id
	});

	const characterDamageData = $state<CharacterDamage>({
		minDamage: 0,
		maxDamage: 0,
		averageDamage: 0
	});

	function handleAddDamage() {
		const result = calculateDamage(characterAttributeData);
		characterDamageData.minDamage = result.minDamage;
		characterDamageData.maxDamage = result.maxDamage;
		characterDamageData.averageDamage = result.averageDamage;
		request.push({
			...characterAttributeData,
			...characterDamageData
		});
	}

	async function handleSave() {
		const response = (await clientFetch('/', Actions.createCharacterAttribute, request)) as Result;
		console.log(response);
		if (
			!response.isSuccess &&
			response.fieldErrors != null &&
			Object.keys(response.fieldErrors).length > 0
		) {
			fieldErrors = response.fieldErrors;
		}
		// if (response.hasErrors) {
		// 	console.log('has errors');
		// 	console.log((response.errors ?? []).length > 0);
		// 	errors = response.errors ?? [];
		// }
	}

	const damageTypeOptions = Object.values(DamageType)
		.filter((value) => typeof value !== 'string')
		.map((type) => ({
			value: type,
			label: DamageType[type]
		}));

	const weaponTypeOptions = Object.values(WeaponType)
		.filter((value) => typeof value !== 'string')
		.map((type) => ({
			value: type,
			label: WeaponType[type]
		}));
</script>

<!-- {#if errors?.length > 0}
	<Notification {errors.} />
{/if} -->

<Form
	{handleSave}
	data={{ ...characterAttributeData, ...characterDamageData }}
	schema={CharacterAttributeDamageSchema}
>
	<div class="grid grid-cols-3 gap-2">
		<div>
			<Label text="Level" />
			<InputField
				errors={fieldErrors['level']}
				type="number"
				bind:value={characterAttributeData.level}
			/>
		</div>
		<div>
			<Label text="Number of Dice" />
			<InputField
				errors={fieldErrors['numDice']}
				type="number"
				bind:value={characterAttributeData.numDice}
			/>
		</div>
		<div>
			<Label text="Die Size" />
			<InputField
				errors={fieldErrors['dieSize']}
				type="number"
				bind:value={characterAttributeData.dieSize}
			/>
		</div>
	</div>
	<div class="grid grid-cols-3 gap-2">
		<div>
			<Label text="Strength Modifier" />
			<InputField
				errors={fieldErrors['strengthModifier']}
				type="number"
				bind:value={characterAttributeData.strengthModifier}
			/>
		</div>
		<div>
			<Label text="Dexterity Modifier" />
			<InputField
				errors={fieldErrors['dexterityModifier']}
				type="number"
				bind:value={characterAttributeData.dexterityModifier}
			/>
		</div>
		<div>
			<Label text="Constitution Modifier" />
			<InputField
				errors={fieldErrors['constitutionModifier']}
				type="number"
				bind:value={characterAttributeData.constitutionModifier}
			/>
		</div>
	</div>
	<div class="grid grid-cols-3 gap-2">
		<div>
			<Label text="Intelligence Modifier" />
			<InputField
				errors={fieldErrors['constitutionModifier']}
				type="number"
				bind:value={characterAttributeData.intelligenceModifier}
			/>
		</div>
		<div>
			<Label text="Wisdom Modifier" />
			<InputField
				errors={fieldErrors['wisdomModifier']}
				type="number"
				bind:value={characterAttributeData.wisdomModifier}
			/>
		</div>
		<div>
			<Label text="Charisma Modifier" />
			<InputField
				errors={fieldErrors['charismaModifier']}
				type="number"
				bind:value={characterAttributeData.charismaModifier}
			/>
		</div>
	</div>
	<div class="grid grid-cols-3 gap-2">
		<div>
			<Label text="Weapon Bonus" />
			<InputField
				errors={fieldErrors['weaponBonus']}
				type="number"
				bind:value={characterAttributeData.weaponBonus}
			/>
		</div>
		<div>
			<Label text="Damage Type" />
			<SelectField
				errors={fieldErrors['damageType']}
				bind:value={characterAttributeData.damageType}
				options={damageTypeOptions}
			/>
		</div>
		<div>
			<Label text="Weapon Type" />
			<SelectField
				errors={fieldErrors['weaponType']}
				bind:value={characterAttributeData.weaponType}
				options={weaponTypeOptions}
			/>
		</div>
	</div>
	<div class="grid grid-cols-2 gap-2">
		<div>
			<Label text="Extra Attack" />
			<InputField
				errors={fieldErrors['extraAttack']}
				type="number"
				bind:value={characterAttributeData.extraAttack}
			/>
		</div>
		<div class="flex justify-end gap-2">
			<div class="mt-2 flex items-center gap-2">
				<Label text="Critical Hit" />
				<InputField
					errors={fieldErrors['criticalHit']}
					type="checkbox"
					bind:checked={characterAttributeData.criticalHit}
				/>
			</div>
			<div class="mt-2 flex items-center gap-2">
				<Label text="Advantage" />
				<InputField
					errors={fieldErrors['advantage']}
					type="checkbox"
					bind:checked={characterAttributeData.advantage}
				/>
			</div>
		</div>
	</div>
	<div class="mb-10 mt-5 flex justify-end">
		<TextButton onClick={handleAddDamage} text="Show Damage" />
	</div>
	<div class="grid grid-cols-3 gap-2">
		<div>
			<Label text="Min Damage" />
			<InputField
				errors={fieldErrors['minDamage']}
				readonly={true}
				type="number"
				bind:value={characterDamageData.minDamage}
			/>
		</div>
		<div>
			<Label text="Max Damage" />
			<InputField
				errors={fieldErrors['maxDamage']}
				readonly={true}
				type="number"
				bind:value={characterDamageData.maxDamage}
			/>
		</div>
		<div>
			<Label text="Average Damage" />
			<InputField
				errors={fieldErrors['averageDamage']}
				readonly={true}
				type="number"
				bind:value={characterDamageData.averageDamage}
			/>
		</div>
	</div>
</Form>
