export function clickOutside(
	node: HTMLElement,
	params: { callback: () => void; triggerButtonId: string }
) {
	const { callback, triggerButtonId } = params;

	const handleClick = (event: MouseEvent) => {
		const target = event.target as Node;
		const triggerElement = document.getElementById(params.triggerButtonId);

		if (triggerElement && triggerElement.contains(target)) {
			return;
		}

		if (node && !node.contains(target) && !event.defaultPrevented) {
			callback();
		}
	};

	const handleKeydown = (event: KeyboardEvent) => {
		if (event.key === 'Escape') {
			callback();
		}
	};

	document.addEventListener('click', handleClick, true);
	document.addEventListener('keydown', handleKeydown, true);

	return {
		destroy() {
			document.removeEventListener('click', handleClick, true);
			document.removeEventListener('keydown', handleKeydown, true);
		}
	};
}
