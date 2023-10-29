using Casual.BI.LLM;
using Casual.BI.LLM.ContextManagers;

const string text = @"The early morning sun crept over the horizon, casting long shadows across the rippling grass. A light breeze caused the blades of grass to sway gently, the meadow coming to life with the dawn. In the distance, the faint sound of a babbling brook could be heard as it wound its way through the valley. Surrounding the meadow were rolling tree-covered hills that seemed to stretch on endlessly.

As the sun rose higher in the clear blue sky, the meadow was bathed in golden light. Dewdrops nestled on each blade of grass began to sparkle like tiny diamonds. The sweet scent of honeysuckle and lavender hung in the air, released as the sun warmed the earth. Birds began their morning songs, flitting from tree to tree in search of a morning meal.

Near the edge of the meadow stood a mighty oak, likely hundreds of years old. Its gnarled trunk was thick and twisted, covered in grooves and hollows. Sturdy branches stretched outward, leafy twigs rustling when brushed by the breeze. This venerable oak had stood watch over the meadow for so long that it had become part of the landscape itself, as integral to the scene as the bubbling brook or the rolling hills.

As the day wore on, the sun rose higher in the sky until it was directly overhead. The meadow became alive with insects dancing and buzzing between the flowers. Butterflies of all colors floated gracefully on the breeze, their wings shimmering in the sunlight. Bees and other pollinators busily moved about their work, gathering nectar and transferring pollen from flower to flower. Their collective buzz was like a symphony of nature.

Rabbits cautiously hopped out from the edge of the forest, nibbling on grass and wildflowers as they kept a watchful eye out for predators. Birds swooped down from the skies to snatch up insects on the fly, carrying them back to feed hungry chicks waiting in their nests. It was a lively summer scene, with the plants, animals, insects, and birds each playing an integral role in the meadow's ecosystem.

As midday arrived, the creatures retreated from the heat, seeking shade under trees and bushes. The meadow became still and quiet, save for the occasional grasshopper jumping through the grass or butterfly fluttering by. The air was fragrant with the smell of warmed earth and grass. Only the hardy wildflowers still bloomed under the intense summer sun.

Slowly the sun crawled across the sky, the long shadows gradually returning. The wise old oak cast a large pool of shade, inviting respite from the heat. As the sun sank lower, the creatures stirred once more, resuming their daily activities. Flowers unfurled as the lowering angle of the light.

Finally, the sun dipped below the horizon, giving way to dusk. The sky turned a blend of oranges and purples as the last rays of light faded. One by one, stars blinked into view overhead as twilight descended. The moon rose, a silver crescent low in the darkening sky. As darkness fell, the chirping of crickets and night insects replaced the songs of the daytime birds. The meadow took on a magical quality under the glow of moonlight.

The cycle would begin again at dawn, just as it had each day for longer than human memory. But for now, the meadow rested in silence and moonlight, a picture of natural serenity.";

Console.WriteLine(new string('-',50));
Console.WriteLine("Looping");
Console.WriteLine(new string('-',50));

var loopingContextManagementStrategy =
    new LoopingContextManagementStrategy("gpt-3.5-turbo",
        "your response should include everything from the previous response, but edit it and add new information as you see fit",
        100);

var response = await ChatRequest.Create(loopingContextManagementStrategy)
    .WithPersona("You are a detailed note taker")
    .WithMessage("user",text)
    .WithMessage("user","what creatures are there? be as terse as possible, respond only with a comma separated list of creatures.","prompt")
    .Send();
Console.WriteLine($"looping response: {response}");

Console.WriteLine(new string('-',50));
Console.WriteLine("Summary");
Console.WriteLine(new string('-',50));

var summaryContextManager = new SummaryContextManagementStrategy("gpt-3.5-turbo", 100);

response = await ChatRequest.Create(summaryContextManager)
    .WithPersona("You are an astute observer of creatures.  it is the main thing you care about, and anything creature related is important to you.")
    .WithMessage("user",text)
    .WithMessage("user","what creatures are there? be as terse as possible, respond only with a comma separated list of creatures.","prompt")
    .Send();

Console.WriteLine($"summary response: {response}");
