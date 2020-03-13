Simple inventory system using ScriptableObjects and JSON
By Øivin Fjeldstad Madsen http://www.oivin.no/

There is a quirk about saving (serializing) ScriptableObjects in Unity. I explain in comments where relevant in the scripts, however this is a summary:

A ScriptableObject is just a data container. We use it to store various variables that we don't need to change during gameplay.
Think for example the card values in HearthStone - we don't ever change those during gameplay.

Now this little program deals with creating inventory items as ScriptableObjects, and then saving a player inventory using JSON serialization.
JSON is just a text file with a specific syntax that contains variables, much like a C# or JavaScript class, but without any logic (just data).

Now, we want to save just a reference to the item in JSON so we can get the values from the ScriptableObject container when we load it.
If we saved all the data from the ScriptableObject it would defeat the purpose of having them.

Unity doesn't do this by default, which is why we have a "hash table" that can convert the saved values in JSON to a reference to our ScriptableObject,
and by extent, its data.

See the comments in the code for more.