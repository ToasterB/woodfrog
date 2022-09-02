import numpy

pattern = [[1, -2], [2, -1], [2, 1], [1, 2], [-1, 2], [-2, 1], [-2,-1], [-1, -2]]
testList = []

outfile = open('knightPattern.txt', 'w+')
i = 0

for row in range(0,8):
    for col in range(0,8):
        output = numpy.zeros(64, dtype=int)
        
        for move in pattern:
            if (0 <= row + move[0] < 8) and (0 <= col + move[1] < 8):
                output[(row + move[0]) * 8 + (col + move[1])] = 1
        
        numberString = hex(int(''.join(str(bit) for bit in output)[::-1], 2))
        testList.append(numberString)

        outfile.write(numberString)
        for x in range(18-len(numberString)):
            outfile.write(' ') 
        outfile.write(', ')

        i += 1
        if(i == 8):
            i = 0
            outfile.write('\n')

        
