from bitarray import bitarray
offset = 4000
#TUTAJ NARAZIE WPISUJEMY TO Z ŁAPY TO CO OTRZYMALISMY PRZY ZAKODOWYWANIU
length = 88
bit_array = bitarray()
with open("out.png","rb") as read_file:
    i=0
    byte_index =0
    while byte := read_file.read(1):
        if byte_index >= offset:
            b = byte[0]
            b = (b & 0b00000001)
            bit_array.append(b)
            i += 1
        byte_index +=1
        if i >= length:
            break
print(bit_array)
byte_data = bit_array.tobytes()
text = byte_data.decode('utf-8')
print(text)