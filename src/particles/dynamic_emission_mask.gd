extends GPUParticles2D

@export var sprite_sheet : Texture2D
@export var frame_coords : Vector2 = Vector2(0.0, 0.0)
@export var frame_size : Vector2 = Vector2(0.0, 0.0)

@export var sprite : Sprite2D

func _ready():
    sprite = owner.get_node("Sprite2D")
    sprite.connect("texture_changed", update_mask)


func update_mask():
    sprite_sheet = sprite.texture
    frame_coords = sprite.frame_coords
    var size = sprite_sheet.get_size()
    frame_size = Vector2(size.x / float(sprite.hframes), size.y / float(sprite.vframes))

    var image : Image = sprite_sheet.get_image()

    var region_rect = Rect2(
        frame_coords.x * frame_size.x,
        frame_coords.y * frame_size.y,
        frame_size.x, frame_size.y)

    if !Rect2(Vector2.ZERO, image.get_size()).encloses(region_rect):
        push_error(frame_coords, frame_size, size, region_rect, image.get_size())

    var frame_image : Image = image.get_region(region_rect)

    var frame_texture = ImageTexture.create_from_image(frame_image)

    var process_material : ParticleProcessMaterial = self.process_material
    if process_material:
        process_material.emission_shape = ParticleProcessMaterial.EMISSION_SHAPE_POINTS
        process_material.emission_point_texture = frame_texture
    else:
        push_error("no process material")
