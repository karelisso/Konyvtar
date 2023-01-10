cpu_hover_update(); //put this at the VERY TOP of the file.

//css_update.gml
//this script is used for anything you want to display on the CSS in real-time
//NOTE: CSS is short for Character Select Screen

css_char_anim_time ++; //animates the css by counting up, similarly to a state_timer
css_anim_time ++; //animates the css by counting up, similarly to a state_timer
alt_cur = get_player_color(player); //checks the current alt
alt_fix = player; //keep this line, on the online css the player is actually player 0, we later go on init_shader to check for this variable

//if the current alt isn't the same as the previous one, reset the animation timer and update the previous alt
if (alt_prev != alt_cur)
{
    css_anim_time = 0;
    alt_prev = alt_cur;
}


if(css_anim_time == 0){
	if(random_func(1, 5, true) == 1){
		css_char_pic = sprite_get("charselect_shirtless");
		sound_play(sound_get("ShirtTear"));
		nude_alt = true;
    	if (voice_button == 0)
    	{
        	voice_button = 2;
    	}
    	if (voice_button == 1)
    	{
        	voice_button = 3;
    	}
	}else{
		css_char_pic = sprite_get("charselect");
		nude_alt = false;
    	if (voice_button == 3)
    	{
        	voice_button = 1;
    	}
    	if (voice_button == 2)
    	{
        	voice_button = 0;
    	}
	   
	}
}

//rainbow alt stuff
if (alt_cur == 28) user_event(0);


var voice_button_pos = 
[
    x + voice_button_x,
    y + voice_button_y,
    x + voice_button_x+30,
    y + voice_button_y+26,
]

if get_synced_var(player) > 3 {
	set_synced_var( player, 0)
}

var voice_button_index = voice_button*3;

cursor_x = get_instance_x(cursor_id);
cursor_y = get_instance_y(cursor_id);

if (cursor_x > voice_button_pos[0] && cursor_x < voice_button_pos[2] && cursor_y > voice_button_pos[1] && cursor_y < voice_button_pos[3] && !instance_exists(oTestPlayer))
{
	suppress_cursor = true;
   if (menu_a_pressed){
       voice_button_index++;
       sound_play(asset_get("mfx_option"));
       if (voice_button == 0 || voice_button == 2)
       {
       	   if( nude_alt == true){
        		voice_button = 3;
       	   }else{
       	   		voice_button = 1;
       	   }
           
           sound_stop(sound_get("VoiceMSoul01"));
		   sound_play(sound_get(chance[random_func(0, 5, true)]));
       }
       else if (voice_button == 1 ||  voice_button == 3)
       {
       	   if( nude_alt == true){
        		voice_button = 2;
       	   }else{
       	   		voice_button = 0;
       	   }
       	   
           sound_stop(sound_get("VoiceMSoul01"));
           sound_play(sound_get("VoiceMSoul01"));
       }
   }
   voice_button_index++;
   //voice_button_index = voice_button_index_draw
}else {
    suppress_cursor = false;
}




















//as always, #defines go at the bottom of the script.
#define cpu_hover_update()
var p = player;
var is_cpu = (get_player_hud_color(p) == 8421504);

if (is_cpu) {
    var pb = plate_bounds, cs = cursors;
    if (cpu_is_hovered) {
        //suppress_cursor = true;
        var c = cs[@cpu_hovering_player]
        cursor_id = c;
        var cx = get_instance_x(c),
            cy = get_instance_y(c);
        if (cpu_hover_time < 10) cpu_hover_time++;
        if (cpu_color_swap_time < 5) cpu_color_swap_time++;
        if (cx != clamp(cx, pb[0],pb[2]) || cy != clamp(cy, pb[1],pb[3])) {
            cpu_is_hovered = false;
            c = cs[@p];
            cursor_id = c;
        }
    } else {
        //suppress_cursor = false;
        var hplayer = get_new_hovering_player();
        if (cpu_hover_time > 0) cpu_hover_time--;
        if (hplayer != -1) {
            cpuh_new_color = get_player_hud_color(hplayer);
            if (cpu_hover_time > 0) {
                cpuh_prev_color = get_player_hud_color(cpu_hovering_player);
                cpu_color_swap_time = 0;
            } else { //if the player indicator is not being displayed already
                cpuh_prev_color = cpuh_new_color;
                cpu_color_swap_time = 10;
            }
            cpu_is_hovered = true;
            cpu_hovering_player = hplayer;
            cursor_id = cs[@hplayer];
        }
    }
}

voice_button_x = 180 - 16*is_cpu
voice_button_y = 75

#define get_new_hovering_player()
var pb = plate_bounds, cs = cursors;
suppress_cursor = true;
for (var i = 1; i <= 4; i++) {
    var c = cs[@i];
    var cx = get_instance_x(c);
    var cy = get_instance_y(c);
    if cx == clamp(cx, pb[@0], pb[@2]) && cy == clamp(cy, pb[@1], pb[@3]) {
        return i;
    } 
}
return -1;