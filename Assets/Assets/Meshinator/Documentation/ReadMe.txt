Meshinator - Real-time Mesh Deformation Tool

Foreword
----------------------------------------
Thank you for purchasing Meshinator! This tool will help you quickly handle compressing and fracturing of your meshes in realtime. If you have any comments or suggestions, please email me at seraph144@gmail.com. I hope you find this tool to be as useful as I have in my projects!


What your GameObject needs to use Meshinator
----------------------------------------
Using Meshinator to make your meshed GameObjects respond to physical forces is very simple. First, your game object will need two components:

1) A MeshFilter - We need a mesh to deform
2) A RigidBody - Meshinator primarily relies on physical collisions (and the forces they generate) to determine how to deform your mesh

Additionally, you will most likely want the following components (although they are not strictly required):

1) A Collider - Without a collider, you would have any physical collisions happening, so your mesh won't be deformed by Meshinator. In my example scenes, I'm using MeshColliders. These are great for objects that end up taking multiple hits over their lifetime, and whose geometry should respond realistically to each impact. However, there are some cases in which other collider types (such as a BoxCollider) would be better. For instance, if you want a race car to take superficial damage as it bumps into walls, without totally deforming the car beyond recognition, a BoxCollider would limit the impact forces of a collision on the geometry.


Using Meshinator - Getting Started
----------------------------------------
To use Meshinator, all you need to do is add the "Meshinator" script to your GameObject. That's it! Now your object's mesh will be deformed by physical impacts as it collides with other objects in your scene.


Using Meshinator - The Basics
----------------------------------------
Once you have the Meshinator script attached to your object, you will most likely want to tweak some of its parameters to get the kind of behavior you want.

Cache Option: During the mesh deformation process, there are three large performance costs: Loading the mesh data into a structure we can modify on the fly, actually modifying the mesh, and recomputing the MeshCollider (if you have one). The Cache Option field gives you some options on mitigating the first cost (loading the mesh data into a structure we can modify on the fly).
	+ None - This will load up the mesh data on every mesh deformation and remove it once the mesh deformation is done. This option has the lowest memory footprint, but requires an extra computation hit on each mesh deformation. This is a good option when operating in a low memory environment, and the computational cost of the mesh deformation isn't a big problem.
	+ CacheAfterCollision - This will keep an up-to-date copy of the mesh information stored away after the first mesh deformation. This means that once an object's mesh has been deformed once, that object will have a larger memory footprint, but all future collisions will not have the pay the upfront cost of loading the mesh data. This is a good option if you have a lot of objects in a scene that COULD be deformed, but only expect a small number of them to be.
	+ CacheOnLoad - This loads the mesh data up at the start of the scene, so that even the first mesh deformation is as quick as possible. Of the three possible Cache Options, this one is the most likely to have a larger memory footprint, and could cause the loading of the scene to take longer. However, all collisions will happen with less computational cost. This is good when load times and memory aren't a huge concern, but computational power is a concern.
	
Impact Shape: This is the shape of the impact resulting from physical collsions.
	+ FlatImpact - This will create a flat impact surface. This is suggested for the "Fracture" ImpactType (mentioned below) and for small objects that are hitting larger objects or environmental objects (like walls).
	+ SphericalImpact - This will create a sphere-like impact area. This is suggested for the "Compression" ImpactType when small objects (like bullets) are impacting a larger object. It is suggested that you NOT use this ImpactType with the "Fracture" ImpactType (mentioned below) due to the likelihood of colliders overlapping after the GameObject is broken into two pieces. This can cause very weird phyics calculations for Unity.

Impact Type: This is the type of mesh deformation that will occur on a physical collision.
	+ Compression - This will compress the mesh away from the point of impact, in the direction of the impact force. This results in the mesh being "dented" or compressed.
	+ Fracture - This will divide the mesh into two pieces. The separation will occur along surface of the impact area created by the Impact Shape (mentioned above). This results in the creation of a new GameObject, and both the new GameObject, and the old one will have their RigidBodies adjusted (mass, center of mass, velocity, etc.) to take into account the separation. This ImpactType can cause the physics system in Unity to be tempermental, especially with certain types of colliders, so thorough testing objects with this Impact Type is a good idea.
	
Force Resistance: This is the amount of force this object can resist before any deformation occurs. Setting this to a high value will protect your object from being deformed by tiny impacts.

Max Force Per Impact: This is the maximum amount of force that will be applied to deform an object's mesh. For any deformation to occur, this value MUST be great than the Force Resistance (mentioned above). Setting this value close to the Force Resistance can protect a mesh from being overly deformed by large impacts (such as pounding a sphere flat from a long fall).

Force Multiplier: This value is multiplied by the force after Force Resistance and Max Force Per Impact are taken into account. This allows you to scale up or down the amount of deformation that occurs from impacts. NOTE: This value MUST be equal to or greater than zero!


Common Issues / FAQ
----------------------------------------
Q) It seems like ANY impact completely mashes my mesh... How can I scale things back a bit?
A) You have two options here. You can either increase the Force Resistance (making the object require larger forces to register as a deformation-worthy impact), or decrease the Force Multiplier (which scales down the force of the impact as it is applied to the mesh).

Q) After a Fracture, my objects shoot off at crazy-high speeds! What gives?!
A) What is probably happening is that the collider for your old object and the new object created by the fracture are overlapping when the new object is created, which really comfuses Unity's physics engine... Consider using a more precise collider on your object, such as a MeshCollider. If you are already using a MeshCollider that is Convex, try turning the Convex option off at least temporarily on an impact. This issue was the biggest problem I had when using the "Fracture" Impact Type.


Performance Notes
----------------------------------------
There are three main CPU hits when deforming an object's mesh:
1) Loading the mesh data into a structure we can modify on the fly
2) Actually modifying the mesh
3) Recomputing the MeshCollider (if you have one).

The first can be mitigated (at the expense of a larger memory footprint) by using a different Cache Option (mentioned above in "Using Meshinator - The Basics"). The second is done on a background thread, so it shouldn't impact your main thread. However, if this is taking too long, consider making your mesh geometry less complex. You could also try to make sure that you have fewer deformation calculations going on at one time. The third can be mitigated by using other types of colliders (in some cases), or writing custom code to stagger the recomputation of the MeshColliders (which would be a small project in itself).


Final Word
----------------------------------------
Again, thank you for purchasing Meshinator. I hope you find this tool very useful, and if you have any issues or suggestions, please let me know. Even (especially!) if you have some code tweaks that you think make this work better, please let me know!


Contact
----------------------------------------
Mike Mahoney
seraph144@gmail.com